namespace ZulaMed.API.Endpoints.PlaylistRestApi.Delete;

public static class Mapper
{
    public static DeletePlaylistCommand ToCommand(this Request request)
    {
        return new DeletePlaylistCommand()
        {
            Id = request.Id
        };
    }
}