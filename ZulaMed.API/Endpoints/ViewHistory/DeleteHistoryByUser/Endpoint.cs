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
        Delete("/viewHistory/owner/{ownerId}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        if (userId != req.OwnerId)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        var result = await _mediator.Send(new DeleteHistoryByUserCommand
        {
            OwnerId = req.OwnerId,
        }, ct);
        if (result)
        {
            await SendOkAsync(ct);
            return;
        }
        await SendNotFoundAsync(ct);
    }
}