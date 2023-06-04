using Amazon.S3.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ZulaMed.VideoConversion.Infrastructure;

namespace ZulaMed.VideoConversion.Endpoints.Transcode;

public class Endpoint : IEndpoint
{
    private readonly IQueryHandler<GetVideoFromS3Query, GetObjectResponse> _s3Handler;
    private readonly IQueryHandler<GetVideoResolutionFromVideoQuery, string> _resolutionHandler;

    public Endpoint(IQueryHandler<GetVideoFromS3Query, GetObjectResponse> s3Handler,
        IQueryHandler<GetVideoResolutionFromVideoQuery, string> resolutionHandler)
    {
        _s3Handler = s3Handler;
        _resolutionHandler = resolutionHandler;
    }


    public void ConfigureRoute(IEndpointRouteBuilder builder)
    {
        builder.MapPost("test", Handle)
            .AddEndpointFilter<BodyValidationEndpointFilter<RequestBody>>();
    }


    // I'm allowing empty body because, it's already being handled in the endpoint filter
    private async Task<Results<Ok<string>, BadRequest>> Handle(
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)]
        RequestBody requestBody, CancellationToken token)
    {
        var objectResponse = await _s3Handler.HandleAsync(new GetVideoFromS3Query
        {
            Key = requestBody.S3Path
        }, token);
        await objectResponse.WriteResponseStreamToFileAsync("test.mp4", false, token);
        return Ok(await _resolutionHandler.HandleAsync(new GetVideoResolutionFromVideoQuery()
        {
            PathToFile = "test.mp4"
        }, token));
    }
}