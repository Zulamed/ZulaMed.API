using ZulaMed.API.Domain.User;

namespace ZulaMed.API.Endpoints.UserRestApi.Register;

public static class Mapper
{
    public static CreateUserCommand MapToCommand(this Request request)
    {
        return new CreateUserCommand
        {
            Email = request.Email,
            Login = request.Login,
            Name = request.Name,
            Surname = request.Surname,
            Country = request.Country,
            City = request.City,
            Password = request.Password,
            AccountType = request.AccountType,
        };
    }

    public static UserDTO MapToResponse(this User user)
    {
        return new UserDTO
        {
            Id = user.Id.Value,
            Login = user.Login.Value,
            Email = user.Email.Value,
            Name = user.Name.Value,
            Surname = user.Surname.Value,
            Country = user.Country.Value,
            City = user.City.Value,
            HistoryPaused = user.HistoryPaused.Value,
        };
    }
}
