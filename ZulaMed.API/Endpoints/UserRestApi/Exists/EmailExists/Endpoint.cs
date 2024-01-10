using FastEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;
using static Microsoft.AspNetCore.Http.TypedResults;

namespace ZulaMed.API.Endpoints.UserRestApi.Exists.EmailExists;

public class Endpoint : Endpoint<Request, Results<Ok, NotFound>>
{
    private readonly ZulaMedDbContext _dbContext;

    public Endpoint(ZulaMedDbContext dbContext)
    {
        _dbContext = dbContext;
    }


    public override void Configure()
    {
        Post("/user/email");
        AllowAnonymous();
    }

    public override async Task<Results<Ok, NotFound>> ExecuteAsync(Request r, CancellationToken c)
    {
        var exists = await _dbContext.Set<User>().AnyAsync(x => (string)x.Email == r.Email, c);
        return exists ? Ok() : NotFound();
    }
}

public class Request
{
    public string Email { get; set; } = null!;
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}