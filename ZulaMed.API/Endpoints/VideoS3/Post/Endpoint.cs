using System.Net;
using System.Text.Json;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.SQS;
using Amazon.SQS.Model;
using FastEndpoints;
using Mediator;
using Microsoft.Extensions.Options;

namespace ZulaMed.API.Endpoints.VideoS3.Post;

public class UploadVideoCommandHandler : Mediator.ICommandHandler<UploadVideoCommand, UploadResponse>
{
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<S3BucketOptions> _s3Options;
    private readonly IAmazonSQS _sqs;
    private readonly IOptions<SqsQueueOptions> _sqsOptions;

    public UploadVideoCommandHandler(IAmazonS3 s3Client, IOptions<S3BucketOptions> s3Options,
        IAmazonSQS sqs, IOptions<SqsQueueOptions> sqsOptions)
    {
        _s3Client = s3Client;
        _s3Options = s3Options;
        _sqs = sqs;
        _sqsOptions = sqsOptions;
    }

    public async ValueTask<UploadResponse> Handle(UploadVideoCommand command, CancellationToken cancellationToken)
    {
        var fileExtension = Path.GetExtension(command.Video.FileName);
        var guid = Guid.NewGuid();
        var response = await _s3Client.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _s3Options.Value.BucketName,
            Key = $"videos/{guid}",
            ContentType = command.Video.ContentType,
            InputStream = command.Video.OpenReadStream(),
            Metadata =
            {
                ["x-amz-meta-originalname"] = command.Video.FileName,
                ["x-amz-meta-extension"] = fileExtension
            },
        }, cancellationToken);
        await _sqs.SendMessageAsync(await GetSqsUrl(),
            JsonSerializer.Serialize(new { VideoS3Path = $"videos/{guid}" }), cancellationToken);
        return new UploadResponse
        {
            VideoUrl = _s3Options.Value.BaseUrl + $"/videos/{guid}",
            PutResponse = response
        };
    }
    
    private async Task<string> GetSqsUrl()
    {
        var request = new GetQueueUrlRequest
        {
            QueueName = _sqsOptions.Value.QueueName
        };
        var response = await _sqs.GetQueueUrlAsync(request);
        return response.QueueUrl;
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
        Post("/uploadVideo");
        AllowFileUploads();
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var response = await _mediator.Send(req.MapToCommand(), ct);
        switch (response.PutResponse.HttpStatusCode)
        {
            case HttpStatusCode.OK:
            {
                await SendOkAsync(response.MapToResponse(), ct);
                break;
            }
            default:
                ThrowError("Encountered an error while uploading the video");
                break;
        }
    }
}