using FastEndpoints;
using FluentValidation;
using Mediator;
using OneOf;
using OneOf.Types;

namespace ZulaMed.API.Endpoints.VideoRestApi.GetById;

public class UserDTO
{
    public required Guid Id { get; set; }
    
    public required string Username { get; set; }
    
    public required string? ProfilePictureUrl { get; set; }
    
    public required int Subscribers { get; set; }
}


public class Response
{
    public required VideoDTO Video { get; set; }
    
    public required int NumberOfLikes { get; set; }
    public required UserDTO User { get; set; }
}

public class Request
{
    public required Guid Id { get; init; }
}

public class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}

public class GetVideoByIdQuery : IQuery<OneOf<Response, NotFound>>
{
    public required Guid Id { get; init; }
}