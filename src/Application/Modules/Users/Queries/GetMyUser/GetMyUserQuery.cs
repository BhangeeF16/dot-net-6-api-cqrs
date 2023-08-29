using Application.Modules.Users.Models;
using MediatR;

namespace Application.Modules.Users.Queries.GetMyUser;

public class GetMyUserQuery : IRequest<UserDto>
{

}
