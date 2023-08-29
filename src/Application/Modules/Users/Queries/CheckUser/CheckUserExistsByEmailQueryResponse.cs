using MediatR;

namespace Application.Modules.Users.Queries.CheckUser;

public class CheckUserExistsByEmailQueryResponse : IRequest<bool>
{
    public CheckUserExistsByEmailQueryResponse(bool? doesExist, bool? isPaswordLogin) => (DoesExist, IsPaswordLogin) = (doesExist, isPaswordLogin);

    public bool? DoesExist { get; set; }
    public bool? IsPaswordLogin { get; set; }
}
