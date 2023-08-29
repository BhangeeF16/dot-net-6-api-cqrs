using FluentValidation;
using MediatR;

namespace Application.Modules.Users.Commands.ForgetPassword;

public class ForgetPasswordCommand : IRequest<bool>
{
    public string? Email { get; set; }
    public class Validator : AbstractValidator<ForgetPasswordCommand>
    {
        public Validator()
        {
            RuleFor(c => c.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Email is invalid");
        }
    }
}
