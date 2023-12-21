using ZulaMed.API.Domain.Accounts.PersonalAccount;

namespace ZulaMed.API.Endpoints.UserRestApi.Register.Personal;

public static class Mapper
{
    public static CreatePersonalAccountCommand MapToCommand(this Request request)
    {
        return new CreatePersonalAccountCommand
        {
            Email = request.Email,
            Login = request.Login,
            Password = request.Password,
            Name = request.Name,
            Surname = request.Surname,
            Country = request.Country,
            City = request.City,
            AccountGender = request.AccountGender,
            AccountTitle = request.AccountTitle,
            AccountCareerStage = request.AccountCareerStage,
            AccountProfessionalActivity = request.AccountProfessionalActivity,
            AccountSpecialty = request.AccountSpecialty,
            AccountDepartment = request.AccountDepartment,
            AccountBirthDate = DateOnly.FromDateTime(request.AccountBirthDate),
            AccountInstitute = request.AccountInstitute,
            AccountRole = request.AccountRole,
            PlacesOfWork = request.PlacesOfWork,
        };
    }

    public static PersonalAccountDTO MapToResponse(this PersonalAccount account)
    {
        return new PersonalAccountDTO
        {
            UserId = account.User.Id.Value,
            AccountGender = account.AccountGender.Value == Gender.Male,
            AccountTitle = account.AccountTitle.Value,
            AccountCareerStage = account.AccountCareerStage.Value,
            AccountProfessionalActivity = account.AccountProfessionalActivity.Value,
            AccountSpecialty = account.AccountSpecialty.Value,
            AccountDepartment = account.AccountDepartment.Value,
            AccountBirthDate = account.AccountBirthDate.Value,
            AccountInstitute = account.AccountInstitute.Value,
            AccountRole = account.AccountRole.Value,
            PlacesOfWork = account.PlacesOfWork,
        };
    }
}