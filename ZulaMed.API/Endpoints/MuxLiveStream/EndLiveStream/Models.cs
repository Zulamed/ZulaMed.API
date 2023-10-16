using FastEndpoints;
using FluentValidation;
using OneOf;
using OneOf.Types;

namespace ZulaMed.API.Endpoints.MuxLiveStream.EndLiveStream;


public class Request
{
    public Guid StreamId { get; init; }
}

public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.StreamId)
            .NotEmpty()
            .WithMessage("StreamId is required");
    }
}

public class EndLiveStreamCommand : Mediator.ICommand<OneOf<Success, NotFound, Error<string>>>
{
    public required Guid StreamId { get; init; }


    public required Guid UserId { get; init; }
}
