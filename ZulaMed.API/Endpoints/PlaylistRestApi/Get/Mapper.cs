using ZulaMed.API.Domain.Playlist;

namespace ZulaMed.API.Endpoints.PlaylistRestApi.Get;

public static class Mapper
{
    public static PlaylistDTO ToResponse(this Playlist playlist)
    {
        return new PlaylistDTO
        {
            Id = playlist.Id.Value,
            PlaylistName = playlist.PlaylistName.Value,
            Owner = playlist.Owner,
            Videos = playlist.Videos,
        };
    }
}