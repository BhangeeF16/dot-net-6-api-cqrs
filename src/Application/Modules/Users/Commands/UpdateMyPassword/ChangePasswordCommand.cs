using Domain.Common.Extensions;
using FluentValidation;
using MediatR;

namespace Application.Modules.Users.Commands.UpdateMyPassword
{
    public class ChangePasswordCommand : IRequest<bool>
    {
        public string? OldPassword { get; set; }
        public string? NewPassword { get; set; }
        public string? ConfirmNewPassword { get; set; }
    }
    public class CreateChangePasswordRequestValidator : AbstractValidator<ChangePasswordCommand>
    {
        public CreateChangePasswordRequestValidator()
        {
            RuleFor(c => c.NewPassword).ValidateProperty();
            RuleFor(c => c.ConfirmNewPassword).ValidateProperty();
            RuleFor(c => c.OldPassword).ValidateProperty();
        }
    }
}
