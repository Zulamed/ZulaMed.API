using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Comments;
using ZulaMed.API.Domain.User;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.Comments.SendACommentToVideo;

public class SendCommentToVideoCommand : Mediator.ICommand<OneOf<Comment, NotFound>>
{
    public required Guid VideoId { get; init; }

    public required string Content { get; init; }

    public required Guid SentById { get; init; }
}

public class
    SendCommentToVideoCommandHandler : Mediator.ICommandHandler<SendCommentToVideoCommand, OneOf<Comment, NotFound>>
{
    private readonly ZulaMedDbContext _dbContext;

    public SendCommentToVideoCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async ValueTask<OneOf<Comment, NotFound>> Handle(SendCommentToVideoCommand command,
        CancellationToken cancellationToken)
    {
        var user = await _dbContext.Set<User>()
            .FirstOrDefaultAsync(x => (Guid)x.Id == command.SentById, cancellationToken: cancellationToken);
        var video = await _dbContext.Set<Video>()
            .FirstOrDefaultAsync(x => (Guid)x.Id == command.VideoId, cancellationToken: cancellationToken);
        if (user is null || video is null)
        {
            return new NotFound();
        }

        var comment = new Comment
        {
            Id = (CommentId)Guid.NewGuid(),
            SentAt = (CommentSentDate)DateTime.UtcNow,
            Content = (CommentContent)command.Content,
            SentBy = user,
            RelatedVideo = video
        };
        var entry = await _dbContext.Set<Comment>().AddAsync(comment, cancellationToken);

        video.Comments.Add(comment);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return entry.Entity;
    }
}

public class Endpoint : Endpoint<Request, Response>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/video/{videoId}/comment");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _mediator.Send(new SendCommentToVideoCommand
        {
            VideoId = req.VideoId,
            Content = req.Content,
            SentById = req.SentBy
        }, ct);

        await result.Match(
            s => SendOkAsync(s.ToResponse(), ct),
            nf => SendNotFoundAsync(ct)
        );
    }
}