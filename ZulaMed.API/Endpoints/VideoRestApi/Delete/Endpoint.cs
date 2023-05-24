using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.Delete;

public class DeleteVideoCommandHandler : Mediator.ICommandHandler<DeleteVideoCommand, bool>
{
    private readonly ZulaMedDbContext _dbContext;

    public DeleteVideoCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<bool> Handle(DeleteVideoCommand command, CancellationToken cancellationToken)
    {
        var rows = await _dbContext.Set<Video>().Where(x => command.Id == x.Id)
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
        Delete("/video/{id}");
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