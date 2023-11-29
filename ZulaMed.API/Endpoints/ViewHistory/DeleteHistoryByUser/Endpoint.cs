using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Endpoints.ViewHistory.DeleteHistory;

namespace ZulaMed.API.Endpoints.ViewHistory.DeleteHistoryByUser;

public class DeleteHistoryByUserCommandHandler : Mediator.ICommandHandler<DeleteHistoryByUserCommand, bool>
{
    private readonly ZulaMedDbContext _dbContext;

    public DeleteHistoryByUserCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<bool> Handle(DeleteHistoryByUserCommand byUserCommand, CancellationToken cancellationToken)
    {
        var rows = await _dbContext.Set<Domain.ViewHistory.ViewHistory>().Where(x => byUserCommand.OwnerId == (Guid)x.ViewedBy.Id)
            .ExecuteDeleteAsync(cancellationToken: cancellationToken);
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
        Delete("/viewHistory/owner");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var claim = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
        Guid? userId = null;
        if (claim is not null)
            userId = Guid.Parse(claim);
        var result = await _mediator.Send(new DeleteHistoryByUserCommand
        {
            OwnerId = userId.Value,
        }, ct);
        if (result)
        {
            await SendOkAsync(ct);
            return;
        }
        await SendNotFoundAsync(ct);
    }
}