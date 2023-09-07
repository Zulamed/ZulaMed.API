using Mediator;
using ZulaMed.API.Domain.Playlist;

namespace ZulaMed.API.Endpoints.PlaylistRestApi.Get.GetAll;

public class Response
{
    public required PlaylistDTO[] Playlists { get; init; }
}

public class GetAllPlaylistsQuery : IQuery<Playlist[]>
{
}