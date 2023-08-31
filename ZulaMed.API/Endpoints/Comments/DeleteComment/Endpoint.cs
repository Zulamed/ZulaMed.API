using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Comments;

namespace ZulaMed.API.Endpoints.Comments.DeleteComment;

public class Request
{
    public Guid CommentId { get; set; }
    public Guid VideoId { get; set; }
}


public class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.CommentId).NotEmpty();
        RuleFor(x => x.VideoId).NotEmpty();
    }
}

public class Endpoint : Endpoint<Request>
{
    private readonly ZulaMedDbContext _dbContext;

    public Endpoint(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public override void Configure()
    {
        Delete("video/{VideoId}/comment/{CommentId}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        var rows = await _dbContext.Set<Comment>().Where(x =>
                (Guid)x.Id == req.CommentId
                && (Guid)x.SentBy.Id == userId
                && (Guid)x.RelatedVideo.Id == req.VideoId)
            .ExecuteDeleteAsync(cancellationToken: ct);
        if (rows == 0)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        await SendOkAsync(ct);
    }
}