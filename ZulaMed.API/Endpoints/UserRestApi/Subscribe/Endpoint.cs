using FastEndpoints;
using Mediator;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;

namespace ZulaMed.API.Endpoints.UserRestApi.Subscribe;

public class
    SubscribeCommandHandler : Mediator.ICommandHandler<SubscribeCommand, OneOf<Success, Error<string>, NotFound>>
{
    private readonly ZulaMedDbContext _dbContext;

    public SubscribeCommandHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ValueTask<OneOf<Success, Error<string>, NotFound>> Handle(SubscribeCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
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
        Post("user/{subId}/{userId}/subscribe");
        AllowAnonymous();
    }
}