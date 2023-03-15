using Domain.Models.Auth;
using FluentValidation;
using MediatR;

namespace Application.Modules.Users.Queries.Login
{
    public class LoginQuery : IRequest<UserTokens>
    {
        public string? Email { get; set; }

        public LoginQuery(string? email, string? password)
        {
            Email = email;
            Password = password;
        }

        public string? Password { get; set; }
    }
    public class LoginRequestValidator : AbstractValidator<LoginQuery>
    {
        public LoginRequestValidator()
        {
            RuleFor(c => c.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Email is invalid");
            RuleFor(c => c.Password).NotEmpty().WithMessage("Password is required");
        }
    }
}
