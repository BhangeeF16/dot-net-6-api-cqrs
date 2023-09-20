using Domain.Common.Extensions;
using FluentValidation;
using MediatR;

namespace Application.Modules.Users.Commands.RegisterUser;

public class RegisterUserCommand : IRequest<bool>
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public class Validator : AbstractValidator<RegisterUserCommand>
    {
        public Validator()
        {
            RuleFor(c => c.Email).ValidateProperty().EmailAddress().WithMessage("Email is invalid");
            RuleFor(c => c.FirstName).ValidateProperty();
            RuleFor(c => c.LastName).ValidateProperty();
            RuleFor(c => c.PhoneNumber).ValidateProperty();
        }
    }
}
