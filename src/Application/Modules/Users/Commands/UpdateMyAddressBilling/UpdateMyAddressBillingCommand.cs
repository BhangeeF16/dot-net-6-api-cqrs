using Domain.Common.Extensions;
using FluentValidation;
using MediatR;

namespace Application.Modules.Users.Commands.UpdateMyAddressBilling
{
    public class UpdateMyAddressBillingCommand : IRequest<bool>
    {
        public string PhoneNumber { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }

        public class UpdateMyBillingDetailsCommandValidator : AbstractValidator<UpdateMyAddressBillingCommand>
        {
            public UpdateMyBillingDetailsCommandValidator()
            {
                RuleFor(c => c.PhoneNumber).ValidateProperty();
                RuleFor(c => c.Line1).ValidateProperty();
                RuleFor(c => c.City).ValidateProperty();
                RuleFor(c => c.State).ValidateProperty();
                RuleFor(c => c.PostCode).ValidateProperty();
                RuleFor(c => c.Country).ValidateProperty();
            }
        }
    }
}
