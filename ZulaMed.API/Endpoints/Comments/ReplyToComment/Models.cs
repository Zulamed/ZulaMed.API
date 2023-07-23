using FastEndpoints;
using FluentValidation;

namespace ZulaMed.API.Endpoints.Comments.ReplyToComment;

public class Request 
{
    public Guid ParentCommentId { get; init; }
    
    public Guid VideoId { get; init; }
    
    public required string Content { get; init; }
    
    public Guid SentBy { get; init; } 
}

public class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.ParentCommentId).NotEmpty();
        RuleFor(x => x.VideoId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty();
        RuleFor(x => x.SentBy).NotEmpty();
    }
}