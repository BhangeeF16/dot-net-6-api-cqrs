using Domain.Common.Extensions;
using FluentValidation;
using MediatR;

namespace Application.Modules.Users.Commands.UpdateMyPaymentSource
{
    public class UpdateMyPaymentSourceCommand : IRequest<bool>
    {
        public string? ChargebeeToken { get; set; }
        public UpdateMyPaymentSourceCommand(string? chargebeeToken) => ChargebeeToken = chargebeeToken;

        public class Validator : AbstractValidator<UpdateMyPaymentSourceCommand>
        {
            public Validator() => RuleFor(c => c.ChargebeeToken).ValidateProperty();
        }
    }
}
