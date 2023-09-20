using Domain.Common.Extensions;
using FluentValidation;
using MediatR;

namespace Application.Modules.Users.Commands.UpdateMyPassword;

public class UpdateMyPasswordCommand : IRequest<bool>
{
    public string? OldPassword { get; set; }
    public string? NewPassword { get; set; }
    public string? ConfirmNewPassword { get; set; }

    public class Validator : AbstractValidator<UpdateMyPasswordCommand>
    {
        public Validator()
        {
            RuleFor(c => c.NewPassword).ValidateProperty();
            RuleFor(c => c.ConfirmNewPassword).ValidateProperty();
            RuleFor(c => c.OldPassword).ValidateProperty();

            When(x => !string.IsNullOrEmpty(x.NewPassword) &&!string.IsNullOrEmpty(x.OldPassword) && !string.IsNullOrEmpty(x.ConfirmNewPassword), () =>
            {
                RuleFor(c => c.OldPassword).ValidateProperty().Equal(y => y.ConfirmNewPassword).WithMessage("Old password Cannot Be Same with New Password");
                RuleFor(c => c.NewPassword).ValidateProperty().NotEqual(y => y.ConfirmNewPassword).WithMessage("New password does not match with Confirm Password");
            });
        }
    }
}
