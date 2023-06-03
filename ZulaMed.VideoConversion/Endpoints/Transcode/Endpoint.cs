using Amazon.S3;
using FFMpegCore;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ZulaMed.VideoConversion.Endpoints.Transcode;



public class Endpoint : IEndpoint
{
    private readonly IAmazonS3 _s3;

    public Endpoint(IAmazonS3 s3)
    {
        _s3 = s3;
    }

    public void ConfigureRoute(IEndpointRouteBuilder builder)
    {
        builder.MapPost("test", Handle)
            .AddEndpointFilter<BodyValidationEndpointFilter<RequestBody>>();
    }


    // I'm allowing empty body because, it's already being handled in the endpoint filter
    private async Task<Results<Ok<string>, BadRequest>> Handle([FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] RequestBody requestBody)
    {
        return await Task.FromResult(Ok("string"));
    }
}