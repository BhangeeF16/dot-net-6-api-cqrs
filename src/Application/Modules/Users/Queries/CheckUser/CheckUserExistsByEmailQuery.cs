using FluentValidation;
using MediatR;

namespace Application.Modules.Users.Queries.CheckUser;

public class CheckUserExistsByEmailQuery : IRequest<bool>
{
    public string? Email { get; set; }
    public CheckUserExistsByEmailQuery(string? email) => Email = email;

    public class Validator : AbstractValidator<CheckUserExistsByEmailQuery>
    {
        public Validator() => RuleFor(c => c.Email).NotEmpty().WithMessage("Email is required").EmailAddress();
    }
}
