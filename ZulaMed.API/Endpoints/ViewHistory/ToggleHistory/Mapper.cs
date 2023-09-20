namespace ZulaMed.API.Endpoints.ViewHistory.ToggleHistory;

public static class Mapper
{
    public static ToggleHistoryCommand ToCommand(this Request request)
    {
        return new ToggleHistoryCommand()
        {
            Id = request.Id
        };
    }
}