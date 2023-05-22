using System.Net;
using FastEndpoints;
using FluentValidation;

namespace ZulaMed.API.Endpoints.VideoS3.Delete;

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

public class DeleteResponse
{
    public required HttpStatusCode StatusCode { get; init; }
}

public class DeleteVideoCommand : Mediator.ICommand<DeleteResponse>
{
    public required Guid FileId { get; init; }
}