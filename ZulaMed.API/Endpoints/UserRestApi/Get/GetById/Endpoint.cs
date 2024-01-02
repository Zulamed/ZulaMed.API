using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Get.GetById;

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, Response>
{
    private readonly ZulaMedDbContext _context;
    public GetUserByIdQueryHandler(ZulaMedDbContext context)
    {
        _context = context;
    }
    
    public async ValueTask<Response> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var user = await _context.Set<User>()
            .FirstOrDefaultAsync(x => (Guid)x.Id == query.Id, cancellationToken);
        return new Response { User = user?.ToResponse() };
    }
}

public class Endpoint : Endpoint<Request, Response>
{
    private readonly IMediator _mediator;

    public Endpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/user/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var response = await _mediator.Send(new GetUserByIdQuery
        {
            Id = req.Id
        }, ct);
        if (response.User is null)
        {
            await SendNotFoundAsync(ct);
        }

        await SendAsync(response, cancellation: ct);
    }
}