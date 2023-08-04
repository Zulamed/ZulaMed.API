using FastEndpoints;
using Mediator;
using Microsoft.Extensions.Options;
using ZulaMed.API.Domain.Video;
using ZulaMed.API.Endpoints.VideoRestApi.Get.GetAll;
using ZulaMed.API.Endpoints.VideoRestApi.Get.GetByTitle;

namespace ZulaMed.API.Endpoints.VideoRestApi.Get;

public class Endpoint : Endpoint<Request,Response>
{
    private readonly IMediator _mediator;
    private readonly IOptions<S3BucketOptions> _s3Configuration;

    public Endpoint(IMediator mediator, IOptions<S3BucketOptions> s3Configuration)
    {
        _mediator = mediator;
        _s3Configuration = s3Configuration;
    }

    public override void Configure()
    {
        Get("/video");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req,CancellationToken ct)
    {
        Video[]? videos;

        if (req.Title is not null)
            videos = await _mediator.Send(new GetByTitleQuery { Title = req.Title}, ct);
        else
            videos = await _mediator.Send(new GetAllVideosQuery(), ct);

        await SendAsync(new Response
        {
            Videos = videos.Select(x => x.ToResponse(_s3Configuration.Value.BaseUrl)).ToArray()
        }, cancellation: ct);
    }
}