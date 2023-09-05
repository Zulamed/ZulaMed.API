using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Comments;

namespace ZulaMed.API.Endpoints.Comments.EditComment;

public class Request
{
    public Guid CommentId { get; set; }
    public Guid VideoId { get; set; }
    public required string EditedText { get; set; }
}

public class Response
{
    public required Guid CommentId { get; set; }
    public required string EditedText { get; set; }
}

public class Endpoint : Endpoint<Request, Response>
{
    private readonly ZulaMedDbContext _dbContext;

    public Endpoint(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Patch("/video/{videoId}/comment/{commentId}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        var rows = await _dbContext.Set<Comment>().Where(x =>
                (Guid)x.SentBy.Id == userId &&
                (Guid)x.Id == req.CommentId &&
                (Guid)x.RelatedVideo.Id == req.VideoId)
            .ExecuteUpdateAsync(calls => calls
                .SetProperty(x => x.Content, (CommentContent)req.EditedText), ct);
        
        if (rows == 0)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        await SendAsync(new Response
        {
            CommentId = req.CommentId,
            EditedText = req.EditedText
        }, cancellation: ct);
    }
}