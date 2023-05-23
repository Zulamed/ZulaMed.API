using Mediator;

namespace ZulaMed.API.Endpoints.Video.Get;

public class Response
{
    public required VideoDTO[] Videos { get; init; }
}

public class GetAllVideosQuery : IQuery<Domain.Video.Video[]>
{
}
