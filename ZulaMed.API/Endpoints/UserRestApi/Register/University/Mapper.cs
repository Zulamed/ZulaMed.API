using ZulaMed.API.Domain.Accounts.UniversityAccount;

namespace ZulaMed.API.Endpoints.UserRestApi.Register.University;

public static class Mapper
{
    public static CreateUniversityAccountCommand MapToCommand(this Request request)
    {
        return new CreateUniversityAccountCommand
        {
            AccountAddress = request.AccountAddress,
            AccountPostCode = request.AccountPostCode,
            AccountPhone = request.AccountPhone,
            Email = request.Email,
            Login = request.Login,
            Password = request.Password,
            Name = request.Name,
            Surname = request.Surname,
            Country = request.Country,
            City = request.City,
            AccountUniversity = request.AccountUniversity
        };
    }

    public static UniversityAccountDTO MapToResponse(this UniversityAccount account)
    {
        return new UniversityAccountDTO
        {
            UserId = account.User.Id.Value,
            AccountAddress = account.AccountAddress.Value,
            AccountPostCode = account.AccountPostCode.Value,
            AccountPhone = account.AccountPhone.Value,
            AccountUniversity = account.AccountUniversity.Value
        };
    }
}