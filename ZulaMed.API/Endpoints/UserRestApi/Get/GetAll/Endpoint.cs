using FastEndpoints;
using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Get.GetAll;

public class GetVideoByIdQueryHandler : IQueryHandler<GetAllUsersQuery, User[]>
{
    private readonly ZulaMedDbContext _context;

    public GetVideoByIdQueryHandler(ZulaMedDbContext context)
    {
        _context = context;
    }
    
    public async ValueTask<User[]> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        var users = await _context.Set<User>().ToArrayAsync(cancellationToken: cancellationToken);
        return users;
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
        Get("/user");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var users = await _mediator.Send(new GetAllUsersQuery(), ct);

        await SendAsync(new Response
        {
            Users = users.Select(x => x.ToResponse()).ToArray()
        }, cancellation: ct);
    }
}