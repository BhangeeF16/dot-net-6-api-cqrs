using FluentValidation;
using MediatR;

namespace Application.Modules.Users.Queries.SendOtpForLogin
{
    public class SendOtpForLoginQuery : IRequest<bool>
    {
        public string? Email { get; set; }

        public SendOtpForLoginQuery(string? email)
        {
            Email = email;
        }
    }
    public class SendOtpForLoginQueryValidator : AbstractValidator<SendOtpForLoginQuery>
    {
        public SendOtpForLoginQueryValidator()
        {

            RuleFor(c => c.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress();
        }
    }
}
