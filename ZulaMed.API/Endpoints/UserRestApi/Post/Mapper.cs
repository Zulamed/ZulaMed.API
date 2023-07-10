using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Post;

public static class Mapper
{
    public static CreateUserCommand MapToCommand(this Request request)
    {
        return new CreateUserCommand
        {
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

    public static UserDTO MapToResponse(this User user)
    {
        return new UserDTO
        {
            Id = user.Id.Value,
            Email = user.Email.Value,
            Group = user.Group.Name.Value,
            Name = user.Name.Value,
            Surname = user.Surname.Value,
            Country = user.Country.Value,
            City = user.City.Value,
            University = user.University.Value,
            WorkPlace = user.WorkPlace.Value,
        };
    }
}
