namespace ZulaMed.API.Endpoints.Video.Delete;

public static class Mapper
{
    public static DeleteVideoCommand ToCommand(this Request request)
    {
        return new DeleteVideoCommand()
        {
            Id = request.Id
        };
    }
}