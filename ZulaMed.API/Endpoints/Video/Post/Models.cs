using FastEndpoints;
using FluentValidation;
using OneOf;
using OneOf.Types;
using Vogen;

namespace ZulaMed.API.Endpoints.Video.Post;



public class Request
{
    public required string VideoTitle { get; init; }
    public required string VideoUrl { get; init; }
    public required string VideoThumbnail { get; init; }
    public required string VideoDescription { get; init; }
    public required DateTime VideoPublishedDate { get; init; }
}


public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.VideoTitle).NotEmpty();
        RuleFor(x => x.VideoUrl).NotEmpty();
        RuleFor(x => x.VideoThumbnail).NotEmpty();
        RuleFor(x => x.VideoDescription).NotEmpty();
        RuleFor(x => x.VideoPublishedDate).NotEmpty();
    }
}

public class CreateVideoCommand : Mediator.ICommand<Result<Domain.Video.Video, ValueObjectValidationException>>
{
    public required string VideoTitle { get; init; }
    public required string VideoUrl { get; init; }
    public required string VideoThumbnail { get; init; }
    public required string VideoDescription { get; init; }
    public required DateTime VideoPublishedDate { get; init; }
}
