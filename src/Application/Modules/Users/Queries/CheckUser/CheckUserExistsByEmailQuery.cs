using FluentValidation;
using MediatR;

namespace Application.Modules.Users.Queries.CheckUser
{
    public class CheckUserExistsByEmailQuery : IRequest<CheckUserExistsByEmailQueryResponse>
    {
        public string? Email { get; set; }

        public CheckUserExistsByEmailQuery(string? email)
        {
            Email = email;
        }
    }
    public class CheckUserExistsByEmailRequestValidator : AbstractValidator<CheckUserExistsByEmailQuery>
    {
        public CheckUserExistsByEmailRequestValidator()
        {

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress();
        }
    }
}
