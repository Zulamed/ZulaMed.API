namespace ZulaMed.API.Endpoints.ViewHistory.DeleteHistoryByUser;

public static class Mapper
{
    public static DeleteHistoryByUserCommand ToCommand(this Request request)
    {
        return new DeleteHistoryByUserCommand()
        {
            OwnerId = request.OwnerId
        };
    }
}