using Mediator;
using ZulaMed.VideoConversion.Features.Transcode.Queries;

namespace ZulaMed.VideoConversion.Features.Transcode;

public class Worker : BackgroundService
{
    private readonly IMediator _mediator;
    // private readonly IQueryHandler<GetVideoFromS3Query, GetObjectResponse> _s3Handler;
    //
    // private readonly IQueryHandler<GetVideoResolutionFromVideoQuery, Result<Resolution, InvalidOperationException>>
    //     _resolutionHandler;
    //
    // private readonly ICommandHandler<TranscodeVideoCommand, OneOf<Success<string>, Error>> _transcodeHandler;
    //
    // public Worker(IQueryHandler<GetVideoFromS3Query, GetObjectResponse> s3Handler,
    //     IQueryHandler<GetVideoResolutionFromVideoQuery, Result<Resolution, InvalidOperationException>>
    //         resolutionHandler,
    //     ICommandHandler<TranscodeVideoCommand, OneOf<Success<string>, Error>> transcodeHandler,
    //     ) 
    // {
    //     _s3Handler = s3Handler;
    //     _resolutionHandler = resolutionHandler;
    //     _transcodeHandler = transcodeHandler;
    // }

    public Worker(IMediator mediator)
    {
        _mediator = mediator;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _mediator.Send(new ConsumeMessageFromSqs(), stoppingToken);
            if (response.Messages.Count == 0) 
                continue;
            foreach (var message in response.Messages)
            {
            }
            await Task.Delay(3000, stoppingToken);
        }
    }
}


// public class Endpoint : IEndpoint
// {
//     private readonly IQueryHandler<GetVideoFromS3Query, GetObjectResponse> _s3Handler;
//
//     private readonly IQueryHandler<GetVideoResolutionFromVideoQuery, Result<Resolution, InvalidOperationException>>
//         _resolutionHandler;
//
//     private readonly ICommandHandler<TranscodeVideoCommand, OneOf<Success<string>, Error>> _transcodeHandler;
//
//
//     private readonly Resolution[] _supportedResolutions = {
//         new() {Width = 3840, Height = 2160},
//         new() {Width = 2560, Height = 1440},
//         new() {Width = 1920, Height = 1080},
//         new() { Width = 1280, Height = 720 },
//         new() { Width = 640, Height = 480 },
//         new() { Width = 480, Height = 360 },
//         new() { Width = 320, Height = 240 },
//     };
//
//
//     // need a cqrs processor for the constructor to not look messy like this 
//     public Endpoint(IQueryHandler<GetVideoFromS3Query, GetObjectResponse> s3Handler,
//         IQueryHandler<GetVideoResolutionFromVideoQuery, Result<Resolution, InvalidOperationException>>
//             resolutionHandler,
//         ICommandHandler<TranscodeVideoCommand, OneOf<Success<string>, Error>> transcodeHandler)
//     {
//         _s3Handler = s3Handler;
//         _resolutionHandler = resolutionHandler;
//         _transcodeHandler = transcodeHandler;
//     }
//
//
//     public void ConfigureRoute(IEndpointRouteBuilder builder)
//     {
//         builder.MapPost("test", Handle)
//             .AddEndpointFilter<BodyValidationEndpointFilter<RequestBody>>();
//     }
//
//
//     // I'm allowing empty body because, it's already being handled in the endpoint filter
//     private async Task<Results<Ok<string>, BadRequest<ErrorMessage>>>
//         Handle([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] RequestBody requestBody, CancellationToken token)
//     {
//         var objectResponse = await _s3Handler.HandleAsync(new GetVideoFromS3Query
//         {
//             Key = requestBody.S3Path
//         }, token);
//         await objectResponse.WriteResponseStreamToFileAsync("test.mp4", false, token);
//         var resolutionResponse = await _resolutionHandler.HandleAsync(new GetVideoResolutionFromVideoQuery
//         {
//             PathToFile = "test.mp4"
//         }, token);
//         if (!resolutionResponse.TryPickT0(out var resolution, out var error))
//             return BadRequest(new ErrorMessage
//             {
//                 Message = error.Value.Message,
//                 StatusCode = 400
//             });
//         await _transcodeHandler.HandleAsync(new TranscodeVideoCommand
//         {
//             VideoPath = "test.mp4",
//             Resolution = new Resolution {Width = 1920, Height = 1080}
//         }, token);
//         return Ok("test.mp4");
//     }
// }