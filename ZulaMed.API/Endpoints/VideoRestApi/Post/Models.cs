using FastEndpoints;
using FluentValidation;
using Vogen;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.Post;

public class Response
{
    public required Guid Id { get; init; }
    
    public required string UploadUrl { get; init; } 
    
    public required string MuxPlaybackId { get; init; }
}


public class CreateVideoCommand : Mediator.ICommand<Result<Response, ValueObjectValidationException>>
{
    public required Guid VideoPublisherId { get; init; }
}
