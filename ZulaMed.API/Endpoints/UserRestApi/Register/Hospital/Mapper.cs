using ZulaMed.API.Domain.Accounts.HospitalAccount;

namespace ZulaMed.API.Endpoints.UserRestApi.Register.Hospital;

public static class Mapper
{
    public static CreateHospitalAccountCommand MapToCommand(this Request request)
    {
        return new CreateHospitalAccountCommand
        {
            UserId = request.UserId,
            AccountHospital = request.AccountHospital,
            AccountAddress = request.AccountAddress,
            AccountPostCode = request.AccountPostCode,
            AccountPhone = request.AccountPhone
        };
    }

    public static HospitalAccountDTO MapToResponse(this HospitalAccount account)
    {
        return new HospitalAccountDTO
        {
            UserId = account.User.Id.Value,
            AccountHospital = account.AccountHospital.Value,
            AccountAddress = account.AccountAddress.Value,
            AccountPostCode = account.AccountPostCode.Value,
            AccountPhone = account.AccountPhone.Value
        };
    }
}
