using FastEndpoints;
using FluentValidation;

namespace ZulaMed.API.Endpoints.VideoRestApi.Delete;

public class Request 
{
   public required Guid Id { get; init; } 
}

public class RequestValidator : Validator<Request>
{
   public RequestValidator()
   {
      RuleFor(x => x.Id).NotEmpty();
   }
}


public class DeleteVideoCommand : Mediator.ICommand<bool>
{
   public required Guid Id { get; init; }
   public required Guid UserId { get; init; }
}