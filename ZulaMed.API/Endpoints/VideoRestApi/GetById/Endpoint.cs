using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.GetById;

public class GetVideoByIdQueryHandler : IQueryHandler<GetVideoByIdQuery, Response>
{
    private readonly ZulaMedDbContext _context;

    public GetVideoByIdQueryHandler(ZulaMedDbContext context)
    {
        _context = context;
    }
    
    public async ValueTask<Response> Handle(GetVideoByIdQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var video = await _context.Set<Video>().FirstOrDefaultAsync(x => (Guid)x.Id == query.Id, cancellationToken);
            return new Response { Video = video?.ToResponse() };
        }
        catch (InvalidCastException e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}

public class Endpoint : Endpoint<Request, VideoDTO>
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
        var response = await _mediator.Send(new GetVideoByIdQuery
        {
            Id = req.Id
        }, ct);
        if (response.Video is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        response.Video = response.Video with
        {
            VideoUrl = $"{_s3Configuration.Value.BaseUrl}{response.Video.VideoUrl}"
        };
        await SendAsync(response.Video, cancellation: ct);
    }
}