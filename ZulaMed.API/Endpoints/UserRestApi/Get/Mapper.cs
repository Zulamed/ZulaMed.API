using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Get;

public static class Mapper
{
    public static UserDTO ToResponse(this User user)
    {
        return new UserDTO()
        {
            Id = user.Id.Value,
            City = user.City.Value,
            Country = user.Country.Value,
            Email = user.Email.Value,
            Login = user.Login.Value,
            Group = user.Group.Name.Value,
            Name = user.Name.Value,
            Surname = user.Surname.Value,
            University = user.University.Value,
            WorkPlace = user.WorkPlace.Value
        };
    }
}