using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.GetById;

public class GetVideoByIdQueryHandler : IQueryHandler<GetVideoByIdQuery, OneOf<Response, NotFound>>
{
    private readonly ZulaMedDbContext _context;

    public GetVideoByIdQueryHandler(ZulaMedDbContext context)
    {
        _context = context;
    }

    public async ValueTask<OneOf<Response, NotFound>> Handle(GetVideoByIdQuery query,
        CancellationToken cancellationToken)
    {
        var video = await _context.Set<Video>()
            .Include(x => x.Publisher)
            .FirstOrDefaultAsync(x => (Guid)x.Id == query.Id, cancellationToken); 
        
        if (video is null)
        {
            return new NotFound();
        }

        var subscriberCount = await _context
            .Entry(video.Publisher)
            .Collection(x => x.Subscribers)
            .Query()
            .CountAsync(cancellationToken: cancellationToken);


        return new Response
        {
            Video = video.ToResponse(), User = new UserDTO
            {
                Id = video.Publisher.Id.Value,
                // vogen converts null to "null" string, so we need to check for that
                ProfilePictureUrl = video.Publisher.PhotoUrl.Value == "null" ? null : video.Publisher.PhotoUrl.Value,
                Subscribers = subscriberCount,
                Username = video.Publisher.Login.Value,
            },
            NumberOfLikes = video.VideoLike.Value
        };
    }
}

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
        Get("/video/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var queryResponse = await _mediator.Send(new GetVideoByIdQuery
        {
            Id = req.Id
        }, ct);

        Response? response = null; 
        
        queryResponse.Switch(r => response = r, _ => { });
        if (response is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        response.Video = response.Video with
        {
            VideoUrl = $"{_s3Configuration.Value.BaseUrl}{response.Video.VideoUrl}",
            VideoThumbnail = $"{_s3Configuration.Value.BaseUrl}{response.Video.VideoThumbnail}"
        };
        await SendAsync(response, cancellation: ct);
    }
}