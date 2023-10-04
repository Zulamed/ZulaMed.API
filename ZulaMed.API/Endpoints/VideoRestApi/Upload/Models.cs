using FastEndpoints;
using FluentValidation;
using Mediator;
using OneOf;
using OneOf.Types;

namespace ZulaMed.API.Endpoints.VideoRestApi.Upload;

public class Request 
{

}


public class GetUploadUrlQuery : IQuery<OneOf<Success<Uri>,Error<Exception>>>
{
} 
