using Domain.Models.Auth;
using FluentValidation;
using MediatR;

namespace Application.Modules.Users.Queries.Login
{
    public class LoginQuery : IRequest<UserTokens>
    {
        public LoginQuery(string? email, string? password) => (Email, Password) = (email, password);

        public string? Email { get; set; }
        public string? Password { get; set; }
        
        public class Validator : AbstractValidator<LoginQuery>
        {
            public Validator()
            {
                RuleFor(c => c.Email).NotEmpty().WithMessage("Email is required").EmailAddress().WithMessage("Email is invalid");
                RuleFor(c => c.Password).NotEmpty().WithMessage("Password is required");
            }
        }
    }
}
