namespace ZulaMed.API.Endpoints.PlaylistRestApi.Post;

public static class Mapper
{
    public static PlaylistDTO MapToResponse(this Domain.Playlist.Playlist playlist)
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