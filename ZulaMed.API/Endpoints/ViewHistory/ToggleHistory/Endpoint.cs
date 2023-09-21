using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.ViewHistory.ToggleHistory;

public class ToggleHistoryCommandHandler : Mediator.ICommandHandler<ToggleHistoryCommand, bool>
{
    private readonly ZulaMedDbContext _dbContext;

    public ToggleHistoryCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<bool> Handle(ToggleHistoryCommand command, CancellationToken cancellationToken)
    {
        var rows = await _dbContext.Database.ExecuteSqlAsync(
            $"""UPDATE "User" SET "HistoryPaused" = NOT "HistoryPaused" WHERE "Id" = {command.Id}""",
            cancellationToken: cancellationToken);
        
        return rows > 0;
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
        Put("/viewHistory/{ownerId}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _mediator.Send(req.ToCommand(), ct);
        if (result)
        {
            await SendOkAsync(ct);
            return;
        }
        await SendNotFoundAsync(ct);
    }
}