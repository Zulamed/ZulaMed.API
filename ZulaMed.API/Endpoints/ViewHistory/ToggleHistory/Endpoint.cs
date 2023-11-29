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

public class Endpoint : EndpointWithoutRequest
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Put("/viewHistory");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var claim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
        Guid? userId = null;
        if (claim is not null)
            userId = Guid.Parse(claim);
        var result = await _mediator.Send(new ToggleHistoryCommand
        {
            Id = userId.Value
        }, ct);
        if (result)
        {
            await SendOkAsync(ct);
            return;
        }
        await SendNotFoundAsync(ct);
    }
}