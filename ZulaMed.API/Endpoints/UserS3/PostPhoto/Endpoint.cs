using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserS3.PostPhoto;

public class UploadPhotoCommandHandler : Mediator.ICommandHandler<UploadPhotoCommand, UploadResponse>
{
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<S3BucketOptions> _s3Options;
    private readonly ZulaMedDbContext _dbContext;

    public UploadPhotoCommandHandler(IAmazonS3 s3Client, IOptions<S3BucketOptions> s3Options, ZulaMedDbContext dbContext)
    {
        _s3Client = s3Client;
        _s3Options = s3Options;
        _dbContext = dbContext;
    }

    public async ValueTask<UploadResponse> Handle(UploadPhotoCommand command, CancellationToken cancellationToken)
    {
        var fileExtension = Path.GetExtension(command.Photo.FileName);
        var guid = command.UserId;

        var response = await _s3Client.PutObjectAsync(new PutObjectRequest()
        {
            BucketName = _s3Options.Value.BucketName,
            Key = $"users/images/{guid}",
            ContentType = command.Photo.ContentType,
            InputStream = command.Photo.OpenReadStream(),
            Metadata =
            {
                ["x-amz-meta-originalname"] = command.Photo.FileName,
                ["x-amz-meta-extension"] = fileExtension
            }
        }, cancellationToken);
        var user = await _dbContext.Set<User>().FirstOrDefaultAsync(x => command.UserId == (Guid)x.Id);
        if (user is not null)
        {
            user.PhotoUrl = (PhotoUrl)(_s3Options.Value.BaseUrl + $"users/images/{guid}");
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        return new UploadResponse
        {
            PhotoUrl = _s3Options.Value.BaseUrl + $"users/images/{guid}",
            PutResponse = response
        };
    }
}

public class UploadPhotoEndpoint : Endpoint<Request, Response>
{
    private readonly IMediator _mediator;

    public UploadPhotoEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/user/{id}/photo");
        AllowFileUploads();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        if (userId != req.UserId)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        var response = await _mediator.Send(new UploadPhotoCommand
        {
            Photo = req.Photo,
            UserId = req.UserId
        }, ct);
        
        switch (response.PutResponse.HttpStatusCode)
        {
            case HttpStatusCode.OK:
            {
                await SendOkAsync(new Response()
                {
                    PhotoUrl = response.PhotoUrl
                }, ct);
                break;
            }
            default:
                ThrowError("Encountered an error while uploading the photo");
                break;
        }
    }
}
