using FastEndpoints;
using FluentValidation;
using Mediator;
using ZulaMed.API.Extensions;

namespace ZulaMed.API.Endpoints.ViewHistory.GetComments;

public class Request
{
    public required Guid OwnerId { get; init; }
    
    [QueryParam]
    public int Page { get; init; } = 1;
    
    [QueryParam]
    public int PageSize { get; init; } = 10;
}

public class RequestValidator : Validator<Get.Request>
{
    public RequestValidator()
    {
        RuleFor(x => x.OwnerId)
            .NotEmpty()
            .WithMessage("Id is required");
    }
}

public class Response
{
    public required int Total { get; init; }
    
    public required List<CommentDTO>? Comments { get; init; }
}

public class GetCommentsByUserQuery : IQuery<Response?>
{
    public required Guid OwnerId { get; init; }
    
    public required PaginationOptions PaginationOptions { get; init; }
}

public class CommentDTO
{
    public required Guid Id { get; init; }
    public required string Content { get; init; }
    public required DateTime SentAt { get; init; }
}
