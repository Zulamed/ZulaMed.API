using Mediator;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;
using ZulaMed.API.Domain.Video;

namespace ZulaMed.API.Endpoints.UserRestApi.Get.GetAll;

public class Response
{
    public required UserDTO[] Users { get; init; }
}

public class GetAllUsersQuery : IQuery<User[]>
{
}

