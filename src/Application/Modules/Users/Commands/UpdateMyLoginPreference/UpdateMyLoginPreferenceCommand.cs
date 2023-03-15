using Domain.Common.Extensions;
using FluentValidation;
using MediatR;

namespace Application.Modules.Users.Commands.UpdateMyLoginPreference
{
    public class UpdateMyLoginPreferenceCommand : IRequest<bool>
    {
        public bool IsOtpLogin { get; set; }
        public string? Password { get; set; }
    }
    public class UpdateMyLoginPreferenceCommandValidator : AbstractValidator<UpdateMyLoginPreferenceCommand>
    {
        public UpdateMyLoginPreferenceCommandValidator()
        {
            RuleFor(c => c.IsOtpLogin).ValidateNotNullProperty();
        }
    }
}
