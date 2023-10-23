using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;

namespace ZulaMed.API.Endpoints.ViewHistory.DeleteHistory;

public class DeleteHistoryCommandHandler : Mediator.ICommandHandler<DeleteHistoryCommand, bool>
{
    private readonly ZulaMedDbContext _dbContext;

    public DeleteHistoryCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<bool> Handle(DeleteHistoryCommand byUserCommand, CancellationToken cancellationToken)
    {
        var rows = await _dbContext.Set<Domain.ViewHistory.ViewHistory>().Where(x =>
                byUserCommand.Id == (Guid)x.Id && byUserCommand.UserId == (Guid)x.ViewedBy.Id)
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
        Delete("/viewHistory/history/{Id}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        var result = await _mediator.Send(new DeleteHistoryCommand
        {
            Id = req.Id,
            UserId = userId
        }, ct);
        if (result)
        {
            await SendOkAsync(ct);
            return;
        }

        await SendNotFoundAsync(ct);
    }
}