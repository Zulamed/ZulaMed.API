using System.Runtime.InteropServices;
using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using Vogen;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Comments;
using ZulaMed.API.Domain.User;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.Comments.ReplyToComment;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct ReplyCannotBeParent
{
}

public class ReplyToCommentCommand : Mediator.ICommand<
    OneOf<Success, NotFound, ReplyCannotBeParent>>
{
    public required Guid ParentCommentId { get; init; }

    public required Guid VideoId { get; init; }

    public required string Content { get; init; }

    public required Guid SentBy { get; init; }
}

public class ReplyToCommentCommandHandler : Mediator.ICommandHandler<ReplyToCommentCommand,
    OneOf<Success, NotFound, ReplyCannotBeParent>>
{
    private readonly ZulaMedDbContext _dbContext;

    public ReplyToCommentCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<OneOf<Success, NotFound, ReplyCannotBeParent>> Handle(
        ReplyToCommentCommand command,
        CancellationToken cancellationToken)
    {
        var video = await _dbContext.Set<Video>().SingleOrDefaultAsync(x => (Guid)x.Id == command.VideoId,
            cancellationToken: cancellationToken);
        var parentComment = await _dbContext.Set<Comment>()
            .SingleOrDefaultAsync(x => (Guid)x.Id == command.ParentCommentId, cancellationToken: cancellationToken);
        var user = await _dbContext.Set<User>()
            .SingleOrDefaultAsync(x => (Guid)x.Id == command.SentBy, cancellationToken: cancellationToken);

        if (video is null || parentComment is null || user is null)
            return new NotFound();

        var entity = await _dbContext.Set<Comment>().AddAsync(new Comment
        {
            Id = (CommentId)Guid.NewGuid(),
            Content = (CommentContent)command.Content,
            SentBy = user,
            SentAt = (CommentSentDate)DateTime.UtcNow,
            RelatedVideo = video
        }, cancellationToken);

        if (await _dbContext
                .Set<Reply>()
                .AnyAsync(x => (Guid)x.ReplyComment.Id == command.ParentCommentId,
                    cancellationToken: cancellationToken))
        {
            return new ReplyCannotBeParent();
        }

        await _dbContext.Set<Reply>().AddAsync(new Reply
        {
            ParentComment = parentComment,
            ReplyComment = entity.Entity
        }, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new Success();
    }
}

public class Endpoint : Endpoint<Request>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/video/{videoId}/comment/{parentCommentId}/reply");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        var result = await _mediator.Send(new ReplyToCommentCommand
        {
            ParentCommentId = req.ParentCommentId,
            VideoId = req.VideoId,
            Content = req.Content,
            SentBy = userId
        }, ct);

        await result.Match(
            s => SendOkAsync(ct),
            nf => SendNotFoundAsync(ct),
            rpc =>
            {
                AddError("Reply cannot be parent");
                return SendErrorsAsync(cancellation: ct);
            }
        );
    }
}