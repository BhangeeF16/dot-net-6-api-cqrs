using Application.Modules.Users.Models;
using Domain.Common.Extensions;
using FluentValidation;
using MediatR;

namespace Application.Modules.Users.Commands.RegisterUser
{
    public class RegisterUserCommand : IRequest<bool>
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public int fk_DayID { get; set; }
        public int fk_TimeSlotID { get; set; }
        public string? PostCode { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string? DeliveryInstructions { get; set; }
        public string? UnwantedProducts { get; set; }

        public string? ChargebeeToken { get; set; }

        public long TotalAmount { get; set; } = 0;
        public string? Coupon { get; set; }


        public class Validator : AbstractValidator<RegisterUserCommand>
        {
            public Validator()
            {
                RuleFor(c => c.Email).ValidateProperty().EmailAddress().WithMessage("Email is invalid");
                RuleFor(c => c.FirstName).ValidateProperty();
                RuleFor(c => c.LastName).ValidateProperty();
                RuleFor(c => c.PhoneNumber).ValidateProperty();
                RuleFor(c => c.fk_DayID).ValidateProperty();
                RuleFor(c => c.fk_TimeSlotID).ValidateProperty();
                RuleFor(c => c.PostCode).ValidateProperty();
                RuleFor(c => c.DeliveryDate).ValidateProperty();
                RuleFor(c => c.TotalAmount).ValidateProperty();
            }
        }
    }
}
