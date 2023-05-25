using FastEndpoints;
using FluentValidation;
using Mediator;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.GetByTitle;

public class Request
{
    public required string Title { get; init; }
}

public class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required");
    }
}

public class Response
{
    public required VideoDTO[] Videos { get; init; }
}

public class GetByTitleQuery : IQuery<Video[]>
{
    public required string Title { get; init; }
}