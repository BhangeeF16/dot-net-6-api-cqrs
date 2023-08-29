using Domain.Common.Extensions;
using FluentValidation;
using MediatR;

namespace Application.Modules.Users.Commands.AddAbandonedCartMember;
public class AddAbandonedCartMemberCommand : IRequest<bool>
{
    public string? Email { get; set; }
    public string? PostCode { get; set; }
    public string? FirstName { get; set; }

    public class Validator : AbstractValidator<AddAbandonedCartMemberCommand>
    {
        public Validator()
        {
            RuleFor(c => c.Email).ValidateProperty();
            RuleFor(c => c.PostCode).ValidateProperty();
            RuleFor(c => c.FirstName).ValidateProperty();
        }
    }
}
