namespace ZulaMed.API.Endpoints.UserS3.DeletePhoto;

public static class Mapper
{
    public static DeletePhotoCommand MapToCommand(this Request request)
    {
        return new DeletePhotoCommand()
        {
            FileId = request.Id
        };
    }
}