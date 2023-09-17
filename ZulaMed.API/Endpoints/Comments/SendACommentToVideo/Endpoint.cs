using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Comments;
using ZulaMed.API.Domain.Shared;
using ZulaMed.API.Domain.User;
using ZulaMed.API.Domain.Video;
using ZulaMed.API.Endpoints.Comments.GetCommentsForAVideo;

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
            Id = (Id)Guid.NewGuid(),
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

public class Endpoint : Endpoint<Request, CommentDTO>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Post("/video/{videoId}/comment");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        var result = await _mediator.Send(new SendCommentToVideoCommand
        {
            VideoId = req.VideoId,
            Content = req.Content,
            SentById = userId
        }, ct);

        await result.Match(
            s => SendOkAsync(s.ToDTO(), ct),
            nf => SendNotFoundAsync(ct)
        );
    }
}