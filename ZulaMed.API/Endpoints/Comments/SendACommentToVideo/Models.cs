using FastEndpoints;
using FluentValidation;

namespace ZulaMed.API.Endpoints.Comments.SendACommentToVideo;

public class Request 
{
    public Guid VideoId { get; init; }
    public required string Content { get; init; }
    public required Guid SentBy { get; init; }
}

public class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.VideoId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.SentBy).NotEmpty();
    }
}


public class Response
{
    public required Guid Id { get; init; }
    public required string Content { get; init; }
    public required Guid SentBy { get; init; }
    public required DateTime SentAt { get; init; }
    public required Guid RelatedVideo { get; init; }
}