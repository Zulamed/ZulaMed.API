using FastEndpoints;
using FirebaseAdmin.Auth;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.Auth.SignIn;

public class GetUserByEmailQuery : IQuery<OneOf<User, NotFound>>
{
    public required string Email { get; init; }
}

public class GetUserByEmailQueryHandler : IQueryHandler<GetUserByEmailQuery, OneOf<User, NotFound>>
{
    private readonly ZulaMedDbContext _dbContext;

    public GetUserByEmailQueryHandler(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async ValueTask<OneOf<User, NotFound>> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Set<User>()
            .SingleOrDefaultAsync(x => (string)x.Email == query.Email, cancellationToken);
        if (user is null)
            return new NotFound();
        return user;
    }
}

public class Endpoint : Endpoint<Request>
{
    private readonly FirebaseAuth _auth;
    private readonly IMediator _mediator;
    private readonly IFirebaseApiClient _firebaseClient;
    private readonly IOptions<FirebaseOptions> _options;

    public Endpoint(
        FirebaseAuth auth,
        IMediator mediator,
        IFirebaseApiClient firebaseClient,
        IOptions<FirebaseOptions> options)
    {
        _auth = auth;
        _mediator = mediator;
        _firebaseClient = firebaseClient;
        _options = options;
    }

    public override void Configure()
    {
        Post("user/auth/sign-in");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUserByEmailQuery()
        {
            Email = req.Email
        }, ct);
        
        if (result.TryPickT1(out var notFound, out var user))
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var userData = await _firebaseClient
            .SignInWithPasswordAsync(_options.Value.ApiKey,
                new SignInRequest(req.Email, req.Password));

        if (!userData.IsSuccessStatusCode)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        var token = await _auth.CreateCustomTokenAsync(user.Id.Value.ToString(), ct);
        await SendAsync(new Response(token), cancellation: ct);
    }
}