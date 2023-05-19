using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using FastEndpoints;
using Mediator;
using Microsoft.Extensions.Options;

namespace ZulaMed.API.Endpoints.Video.Post;



public class UploadVideoCommandHandler : Mediator.ICommandHandler<UploadVideoCommand, UploadResponse>
{
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<S3BucketOptions> _s3Options;

    public UploadVideoCommandHandler(IAmazonS3 s3Client, IOptions<S3BucketOptions> s3Options)
    {
        _s3Client = s3Client;
        _s3Options = s3Options;
    }

    public async ValueTask<UploadResponse> Handle(UploadVideoCommand command, CancellationToken cancellationToken)
    {
        var fileExtension = Path.GetExtension(command.Video.FileName);
        var guid = Guid.NewGuid();
        var response = await _s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _s3Options.Value.BucketName,
            Key = $"videos/{guid}",
            InputStream = command.Video.OpenReadStream(),
            Metadata =
            {
                ["x-amz-meta-originalname"] = command.Video.FileName,
                ["x-amz-meta-extension"] = fileExtension
            }
        }, cancellationToken);
        return new UploadResponse
        {
            VideoUrl = _s3Options.Value.BaseUrl + $"/{guid}",
            PutResponse = response
        };
    }
}

public class UploadVideoEndpoint : Endpoint<Request, Response>
{
    private readonly IMediator _mediator;

    public UploadVideoEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/videos");
        AllowFileUploads();
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var response = await _mediator.Send(new UploadVideoCommand
        {
            Video = req.Video
        }, ct);
        switch (response.PutResponse.HttpStatusCode)
        {
            case HttpStatusCode.OK:
            {
                await SendOkAsync(new Response()
                {
                    VideoUrl = response.VideoUrl
                }, cancellation: ct);
                break;
            }
            default:
                ThrowError("Encountered an error while uploading the video");
                break;
        }
    }
}