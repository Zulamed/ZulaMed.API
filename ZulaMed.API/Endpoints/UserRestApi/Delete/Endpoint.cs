using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.UserRestApi.Delete;

public class DeleteUserCommandHandler : Mediator.ICommandHandler<DeleteUserCommand, bool>
{
    private readonly ZulaMedDbContext _dbContext;

    public DeleteUserCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<bool> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var rows = await _dbContext.Set<User>().Where(x => command.Id == (Guid)x.Id && command.UserId == (Guid)x.Id)
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
        Delete("/user/{id}");
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var userId = Guid.Parse(HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")!.Value);
        var result = await _mediator.Send(new DeleteUserCommand
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