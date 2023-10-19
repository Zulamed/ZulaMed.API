using System.Net;
using FastEndpoints;
using FluentValidation;

namespace ZulaMed.API.Endpoints.UserS3.DeletePhoto;

public class Request
{
    public required Guid FileId { get; init; }
}

public class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.FileId)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}

public class DeleteResponse
{
    public required HttpStatusCode StatusCode { get; init; }
}

public class DeletePhotoCommand : Mediator.ICommand<DeleteResponse>
{
    public required Guid FileId { get; init; }
}