using MediatR;

namespace Application.Modules.Users.Queries.CheckUser
{
    public class CheckUserExistsByEmailQueryResponse : IRequest<bool>
    {
        public bool? DoesExist { get; set; }
        public bool? IsPaswordLogin { get; set; }
    }
}
