namespace ZulaMed.API.Endpoints.UserRestApi.Delete;

public static class Mapper
{
    public static DeleteUserCommand ToCommand(this Request request)
    {
        return new DeleteUserCommand()
        {
            Id = request.Id
        };
    }
}