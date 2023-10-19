using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using FastEndpoints;
using Mediator;
using Microsoft.Extensions.Options;

namespace ZulaMed.API.Endpoints.UserS3.DeletePhoto;

public class DeletePhotoCommandHandler : Mediator.ICommandHandler<DeletePhotoCommand, DeleteResponse>
{
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<S3BucketOptions> _options;

    public DeletePhotoCommandHandler(IAmazonS3 s3Client, IOptions<S3BucketOptions> options)
    {
        _s3Client = s3Client;
        _options = options;
    }
    
    
    public async ValueTask<DeleteResponse> Handle(DeletePhotoCommand command, CancellationToken cancellationToken)
    {
        var response = await _s3Client.DeleteObjectAsync(new DeleteObjectRequest()
        {
            BucketName = _options.Value.BucketName,
            Key = $"users/images/{command.FileId}"
        }, cancellationToken);
        return new DeleteResponse
        {
            StatusCode = response.HttpStatusCode
        };
    }
}

public class Endpoint : Endpoint<Request>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Delete("/user/{id}/photo");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        if (userId != req.FileId)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        var response = await _mediator.Send(new DeletePhotoCommand
        {
            FileId = req.FileId,
        }, ct);
        switch (response.StatusCode)
        {
            case HttpStatusCode.NoContent:
                await SendOkAsync(ct);
                break;
            case HttpStatusCode.NotFound:
                await SendNotFoundAsync(ct);
                break;
            default:
                ThrowError($"Couldn't delete the photo. Status Code: {response.StatusCode}");
                break;
        }
    }
}
