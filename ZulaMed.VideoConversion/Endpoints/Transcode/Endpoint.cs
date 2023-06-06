using Amazon.S3.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ZulaMed.VideoConversion.Endpoints.Transcode.Commands;
using ZulaMed.VideoConversion.Endpoints.Transcode.Queries;
using ZulaMed.VideoConversion.Infrastructure;

namespace ZulaMed.VideoConversion.Endpoints.Transcode;

public class Endpoint : IEndpoint
{
    private readonly IQueryHandler<GetVideoFromS3Query, GetObjectResponse> _s3Handler;

    private readonly IQueryHandler<GetVideoResolutionFromVideoQuery, Result<Resolution, InvalidOperationException>>
        _resolutionHandler;

    private readonly ICommandHandler<TranscodeVideoCommand, string[]> _transcodeHandler;


    private readonly Resolution[] _supportedResolutions = {
        new() {Width = 3840, Height = 2160},
        new() {Width = 2560, Height = 1440},
        new() {Width = 1920, Height = 1080},
        new() { Width = 1280, Height = 720 },
        new() { Width = 640, Height = 480 },
        new() { Width = 480, Height = 360 },
        new() { Width = 320, Height = 240 },
    };


    // need a cqrs processor for the constructor to not look messy like this 
    public Endpoint(IQueryHandler<GetVideoFromS3Query, GetObjectResponse> s3Handler,
        IQueryHandler<GetVideoResolutionFromVideoQuery, Result<Resolution, InvalidOperationException>>
            resolutionHandler,
        ICommandHandler<TranscodeVideoCommand, string[]> transcodeHandler)
    {
        _s3Handler = s3Handler;
        _resolutionHandler = resolutionHandler;
        _transcodeHandler = transcodeHandler;
    }


    public void ConfigureRoute(IEndpointRouteBuilder builder)
    {
        builder.MapPost("test", Handle)
            .AddEndpointFilter<BodyValidationEndpointFilter<RequestBody>>();
    }


    // I'm allowing empty body because, it's already being handled in the endpoint filter
    private async Task<Results<Ok<string>, BadRequest<Error>>>
        Handle([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] RequestBody requestBody, CancellationToken token)
    {
        var objectResponse = await _s3Handler.HandleAsync(new GetVideoFromS3Query
        {
            Key = requestBody.S3Path
        }, token);
        await objectResponse.WriteResponseStreamToFileAsync("test.mp4", false, token);
        var resolutionResponse = await _resolutionHandler.HandleAsync(new GetVideoResolutionFromVideoQuery
        {
            PathToFile = "test.mp4"
        }, token);
        if (!resolutionResponse.TryPickT0(out var resolution, out var error))
            return BadRequest(new Error
            {
                ErrorMessage = error.Value.Message,
                StatusCode = 400
            });
        await _transcodeHandler.HandleAsync(new TranscodeVideoCommand
        {
            VideoPath = "test.mp4",
            Resolutions = _supportedResolutions
                .SkipWhile(x => x.Height > resolution.Height && x.Width > resolution.Width)
                .ToArray() 
        }, token);
        return Ok("test.mp4");
    }
}