using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;

namespace ZulaMed.API.Endpoints.VideoRestApi.Like;

public class LikeVideoCommand : Mediator.ICommand<OneOf<Success, Error<string>, NotFound>>
{
    public required Guid VideoId { get; init; }
}

public class
    LikeVideoCommandHandler : Mediator.ICommandHandler<LikeVideoCommand, OneOf<Success, Error<string>, NotFound>>
{
    private readonly ZulaMedDbContext _dbContext;

    public LikeVideoCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public async ValueTask<OneOf<Success, Error<string>, NotFound>> Handle(LikeVideoCommand command,
        CancellationToken cancellationToken)
    {
        try
        {
            var rows = await _dbContext.Database.ExecuteSqlAsync(
                $"""UPDATE "Video" SET "VideoLike" = "VideoLike" + 1 WHERE "Id" = {command.VideoId}""", 
                cancellationToken: cancellationToken);
            return rows > 0 ? new Success() : new NotFound();
        }
        catch (Exception e)
        {
            return new Error<string>(e.Message);
        }
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
        Post("video/{id}/like");
        AllowAnonymous();
        // for some reason FastEndpoints was sending 415, this clears the defaults so that it wouldn't send it 
        Description(b => { }, clearDefaults: true); 
    }


    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _mediator.Send(new LikeVideoCommand { VideoId = req.Id }, ct);
        await result.Match(
            s => SendOkAsync(ct),
            e => SendAsync(new
            {
                StatusCode = 500,
                Message = e.Value
            }, 500, ct),
            n => SendNotFoundAsync(ct)
        );
    }
}