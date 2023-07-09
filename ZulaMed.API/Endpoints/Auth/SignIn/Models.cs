using FastEndpoints;
using FluentValidation;

namespace ZulaMed.API.Endpoints.Auth.SignIn;

public record Request(string Email, string Password);

public class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public record Response(string Token);