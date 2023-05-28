using FastEndpoints;
using Mediator;
using ZulaMed.API.Domain.Video;
using ZulaMed.API.Endpoints.VideoRestApi.Get.GetAll;
using ZulaMed.API.Endpoints.VideoRestApi.Get.GetByTitle;

namespace ZulaMed.API.Endpoints.VideoRestApi.Get;

public class Endpoint : EndpointWithoutRequest<Response>
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

    public override async Task HandleAsync(CancellationToken ct)
    {
        Video[]? videos;

        var queryParam = Query<string>("title", false);

        if (queryParam is not null)
            videos = await _mediator.Send(new GetByTitleQuery { Title = queryParam}, ct);
        else
            videos = await _mediator.Send(new GetAllVideosQuery(), ct);

        await SendAsync(new Response
        {
            Videos = videos.Select(x => x.ToResponse()).ToArray()
        }, cancellation: ct);
    }
}