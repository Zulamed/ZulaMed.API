using Mediator;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.VideoRestApi.Get;

public class Response
{
    public required VideoDTO[] Videos { get; init; }
}

public class GetAllVideosQuery : IQuery<Video[]>
{
}
