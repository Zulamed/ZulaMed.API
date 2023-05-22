namespace ZulaMed.API.Endpoints.VideoS3.Delete;

public static class Mapper
{
    public static DeleteVideoCommand MapToCommand(this Request request)
    {
        return new DeleteVideoCommand()
        {
            FileId = request.Id
        };
    }
}