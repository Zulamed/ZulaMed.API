using System.Net;
using Mediator;

namespace ZulaMed.API.Endpoints.Video.Delete;

public class Request
{
    public required Guid Id { get; init; }
}

public class DeleteResponse
{
    public required HttpStatusCode StatusCode { get; init; }
}

public class DeleteVideoCommand : ICommand<DeleteResponse>
{
    public required Guid FileId { get; init; }
}