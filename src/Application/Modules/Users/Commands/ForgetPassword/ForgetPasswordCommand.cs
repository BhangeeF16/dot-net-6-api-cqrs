using FluentValidation;
using MediatR;

namespace Application.Modules.Users.Commands.ForgetPassword
{
    public class ForgetPasswordCommand : IRequest<bool>
    {
        public string? Email { get; set; }
    }
    public class CreateForgetPasswordRequestValidator : AbstractValidator<ForgetPasswordCommand>
    {
        public CreateForgetPasswordRequestValidator()
        {
            RuleFor(c => c.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Email is invalid");
        }
    }
}
