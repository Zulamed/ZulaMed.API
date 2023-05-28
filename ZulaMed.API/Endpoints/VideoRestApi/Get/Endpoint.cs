using FastEndpoints;
using Mediator;
using ZulaMed.API.Domain.Video;
using ZulaMed.API.Endpoints.VideoRestApi.Get.GetAll;
using ZulaMed.API.Endpoints.VideoRestApi.Get.GetByTitle;

namespace ZulaMed.API.Endpoints.VideoRestApi.Get;

public class Endpoint : Endpoint<Request,Response>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
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
            Videos = videos.Select(x => x.ToResponse()).ToArray()
        }, cancellation: ct);
    }
}