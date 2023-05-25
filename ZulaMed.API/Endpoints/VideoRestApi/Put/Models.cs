using FastEndpoints;
using FluentValidation;
using Vogen;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.Put;

public class Request 
{
    public required string VideoTitle { get; init; }
    
    public required string VideoThumbnail { get; init; }
    
    public required string VideoDescription { get; init; }
}


public class Validator : Validator<Request>
{
    public Validator()
    {
        RuleFor(x => x.VideoTitle).NotEmpty();
        RuleFor(x => x.VideoThumbnail).NotEmpty();
        RuleFor(x => x.VideoDescription).NotEmpty();
    }
}

public class UpdateVideoCommand : Mediator.ICommand<Result<Video, ValueObjectValidationException>>
{
    public required string VideoTitle { get; init; }
    public required string VideoThumbnail { get; init; }
    public required string VideoDescription { get; init; }
}