using FastEndpoints;
using Mediator;
using Microsoft.Extensions.Options;
using ZulaMed.API.Domain.Video;
using ZulaMed.API.Endpoints.VideoRestApi.Get.GetAll;
using ZulaMed.API.Endpoints.VideoRestApi.Get.GetByTitle;
using ZulaMed.API.Endpoints.VideoRestApi.Get.GetByUser;
using ZulaMed.API.Endpoints.VideoRestApi.Get.GetByUserSubscriptions;
using ZulaMed.API.Endpoints.VideoRestApi.Get.GetUserLiked;
using ZulaMed.API.Extensions;

namespace ZulaMed.API.Endpoints.VideoRestApi.Get;

public class Endpoint : Endpoint<Request, Response>
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

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var claim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
        Guid? userId = null;
        if (claim is not null)
            userId = Guid.Parse(claim);
        (Video[] Videos, int Count) videos;
        
        if (req.Title is not null)
            videos = await _mediator.Send(new GetByTitleQuery
            {
                Title = req.Title,
                PaginationOptions = new PaginationOptions(req.Page, req.PageSize)
            }, ct);
        else if (userId is not null && req.Own is not null && req.Own.Value)
        {
            videos = await _mediator.Send(new GetByUserQuery()
            {
                Id = userId.Value,
                PaginationOptions = new PaginationOptions(req.Page, req.PageSize)
            }, ct);
        }
        else if (userId is not null && req.Liked is not null && req.Liked.Value)
            videos = await _mediator.Send(new GetUserLikedQuery()
            {
                UserId = userId.Value,
                PaginationOptions = new PaginationOptions(req.Page, req.PageSize)
            }, ct);
        else if (userId is not null && req.Subscriptions is not null && req.Subscriptions.Value)
            videos = await _mediator.Send(new GetVideosByUserSubscriptionsQuery()
            {
                UserId = userId.Value,
                PaginationOptions = new PaginationOptions(req.Page, req.PageSize)
            }, ct);
        else 
            videos = await _mediator.Send(new GetAllVideosQuery
            {
                PaginationOptions = new PaginationOptions(req.Page, req.PageSize)
            }, ct);
        

        await SendAsync(new Response
        {
            Videos = videos.Videos.Select(x => x.ToResponse(string.Empty)).ToArray(),
            TotalCount = videos.Count
        }, cancellation: ct);
    }
}