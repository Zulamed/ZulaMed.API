namespace ZulaMed.API.Endpoints.PlaylistRestApi.Put;

public static class Mapper
{
    public static UpdatePlaylistCommand MapToCommand(this Request request)
    {
        return new UpdatePlaylistCommand
        {
            PlaylistName = request.PlaylistName,
            PlaylistDescription = request.PlaylistDescription,
            PlaylistId = request.PlaylistId
        };
    }
}