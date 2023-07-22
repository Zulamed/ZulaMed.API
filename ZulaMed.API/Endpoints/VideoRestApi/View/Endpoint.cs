using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.View;

public class ViewCommandHandler : Mediator.ICommandHandler<ViewCommand, OneOf<Success, Error<string>, NotFound>>
{
    private readonly ZulaMedDbContext _dbContext;

    public ViewCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async ValueTask<OneOf<Success, Error<string>, NotFound>> Handle(ViewCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var rows = await _dbContext.Database.ExecuteSqlAsync(
                $"""UPDATE "Video" SET "VideoView" = "VideoView" + 1 WHERE "Id" = {command.Id}""", 
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
        Post("video/{id}/view");
        AllowAnonymous();
        Description(b => { }, clearDefaults: true); 
    }
    
    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _mediator.Send(new ViewCommand { Id = req.Id }, ct);
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