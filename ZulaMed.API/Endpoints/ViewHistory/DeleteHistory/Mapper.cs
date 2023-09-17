namespace ZulaMed.API.Endpoints.ViewHistory.DeleteHistory;

public static class Mapper
{
    public static DeleteHistoryCommand ToCommand(this Request request)
    {
        return new DeleteHistoryCommand
        {
            Id = request.Id
        };
    }
}