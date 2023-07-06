namespace ZulaMed.API.Endpoints.UserRestApi.Put;

public static class Mapper
{
    public static UpdateUserCommand MapToCommand(this Request request)
    {
        return new UpdateUserCommand
        {
            Id = request.Id,
            Email = request.Email,
            GroupId = request.GroupId,
            Name = request.Name,
            Surname = request.Surname,
            Country = request.Country,
            City = request.City,
            University = request.University,
            WorkPlace = request.WorkPlace,
        };
    } 
}