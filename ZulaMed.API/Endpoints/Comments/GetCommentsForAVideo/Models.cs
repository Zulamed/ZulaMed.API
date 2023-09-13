using FastEndpoints;
using FluentValidation;
using Mediator;
using OneOf;
using OneOf.Types;
using ZulaMed.API.Domain.Comments;
using ZulaMed.API.Extensions;

namespace ZulaMed.API.Endpoints.Comments.GetCommentsForAVideo;

public class Request
{
    public required Guid VideoId { get; init; }
    
    [QueryParam]
    public int Page { get; init; } = 1;
    
    [QueryParam]
    public int PageSize { get; init; } = 10;
}

public class Response
{
    // public required List<Comment>? Comments { get; init; }
    public required int Total { get; init; }
    
    public required List<CommentDTO>? Comments { get; init; }
}

public class RequestValidator : Validator<Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.VideoId)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}

public class GetCommentsForAVideoQuery : IQuery<Response?>
{
    public required Guid VideoId { get; init; }
    
    public required PaginationOptions PaginationOptions { get; init; }
}