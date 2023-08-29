using Application.Common.Private;
using Domain.Common.Extensions;
using FluentValidation;
using MediatR;

namespace Application.Modules.Users.Queries.SendOtpForLogin;

public class SendOtpForLoginQuery : IRequest<bool>
{
    public string? Email { get; set; }
    public SendOtpVia? Via { get; set; }
    public SendOtpForLoginQuery(string? email, int via = 1) => (Email, Via) = (email, via.ToEnum<SendOtpVia>());

    public class Validator : AbstractValidator<SendOtpForLoginQuery>
    {
        public Validator()
        {
            RuleFor(c => c.Email).ValidateProperty().EmailAddress();
            RuleFor(c => c.Via).ValidateEnumProperty();
        }
    }
}
