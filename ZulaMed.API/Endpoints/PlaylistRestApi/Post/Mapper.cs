namespace ZulaMed.API.Endpoints.PlaylistRestApi.Post;

public static class Mapper
{
    public static CreatePlaylistCommand MapToCommand(this Request request)
    {
        return new CreatePlaylistCommand
        {
            OwnerId = request.OwnerId,
            PlaylistName = request.PlaylistName,
            PlaylistDescription = request.PlaylistDescription
        };
    }

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