using FastEndpoints;
using FluentValidation;

namespace ZulaMed.API.Endpoints.Comments.SendACommentToVideo;

public class Request 
{
    public Guid VideoId { get; init; }
    public required string Content { get; init; }
}

public class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.VideoId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
    }
}
