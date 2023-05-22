using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using FastEndpoints;
using Mediator;
using Microsoft.Extensions.Options;

namespace ZulaMed.API.Endpoints.Video.Delete;


public class DeleteVideoCommandHandler : Mediator.ICommandHandler<DeleteVideoCommand, DeleteResponse>
{
    private readonly IAmazonS3 _s3Client;
    private readonly IOptions<S3BucketOptions> _options;

    public DeleteVideoCommandHandler(IAmazonS3 s3Client, IOptions<S3BucketOptions> options)
    {
        _s3Client = s3Client;
        _options = options;
    }
    
    
    public async ValueTask<DeleteResponse> Handle(DeleteVideoCommand command, CancellationToken cancellationToken)
    {
        var response = await _s3Client.DeleteObjectAsync(new DeleteObjectRequest()
        {
            BucketName = _options.Value.BucketName,
            Key = $"videos/{command.FileId}" 
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
        Delete("/videos/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var response = await _mediator.Send(req.MapToCommand(), ct);
        switch (response.StatusCode)
        {
            case HttpStatusCode.NoContent:
                await SendOkAsync(ct);
                break;
            case HttpStatusCode.NotFound:
                await SendNotFoundAsync(ct);
                break;
            default:
                ThrowError($"Couldn't delete the video. Status Code: {response.StatusCode}");
                break;
        }
    }
}
