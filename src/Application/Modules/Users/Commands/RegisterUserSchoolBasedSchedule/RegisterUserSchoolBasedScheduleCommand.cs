using Application.Modules.Users.Models;
using Domain.Common.Extensions;
using FluentValidation;
using MediatR;

namespace Application.Modules.Users.Commands.RegisterUserSchoolBasedSchedule
{
    public class RegisterUserSchoolBasedScheduleCommand : IRequest<string>
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }

        public int fk_SchoolID { get; set; }

        public int fk_DayID { get; set; }
        public int fk_TimeSlotID { get; set; }
        public DateTime DeliveryDate { get; set; }
        public string? DeliveryInstructions { get; set; }

        public List<int>? UnwantedProduces { get; set; }
        public List<int>? ReplacementProduces { get; set; }

        public string? ChargebeeToken { get; set; }

        public long TotalAmount { get; set; } = 0;
        public string? Coupon { get; set; }

        public AddressRequestModel? Billing { get; set; }
        public PlanVariationRequestModel? PlanVariation { get; set; }
        public List<PantryItemRequestModel>? PantryItems { get; set; }

        public class Validator : AbstractValidator<RegisterUserSchoolBasedScheduleCommand>
        {
            public Validator()
            {
                RuleFor(c => c.Email).ValidateProperty().EmailAddress().WithMessage("Email is invalid");
                RuleFor(c => c.FirstName).ValidateProperty();
                RuleFor(c => c.LastName).ValidateProperty();
                RuleFor(c => c.PhoneNumber).ValidateProperty();
                RuleFor(c => c.fk_SchoolID).ValidateProperty().GreaterThan(0).WithMessage("fk_SchoolID is invalid");
                RuleFor(c => c.fk_DayID).ValidateProperty().GreaterThan(0).WithMessage("fk_DayID is invalid");
                RuleFor(c => c.fk_TimeSlotID).ValidateProperty().GreaterThan(0).WithMessage("fk_TimeSlotID is invalid");
                RuleFor(c => c.DeliveryDate).ValidateProperty();
                RuleFor(c => c.Billing).SetValidator(new AddressRequestModel.Validator());
                RuleFor(c => c.PlanVariation).SetValidator(new PlanVariationRequestModel.Validator());

                When(x => string.IsNullOrEmpty(x.Coupon), () => RuleFor(c => c.TotalAmount).ValidateProperty());

                When(x => x.PantryItems != null && x.PantryItems.Any(), () =>
                {
                    RuleFor(c => c.PantryItems)
                        .ValidateProperty()
                        .Must(x => x.Select(p => p.ID).Distinct().Count() == x.Count).WithMessage("Pantry Items must be unique")
                        .ForEach(y => y.ValidateProperty().SetValidator(new PantryItemRequestModel.Validator()));
                });

                When(x => x.UnwantedProduces != null && x.UnwantedProduces.Any(), () =>
                {
                    RuleFor(c => c.UnwantedProduces)
                        .Must(x => x != null && x.Any() && x.Distinct().Count() <= 4).WithMessage("No more than 4 Unwanted Produces are allowed")
                        .ForEach(y => y.ValidateProperty().GreaterThan(0));

                    When(x => x.ReplacementProduces != null && x.ReplacementProduces.Any(), () =>
                    {
                        RuleFor(c => c.ReplacementProduces)
                        .Must(x => x != null && x.Any() && x.Distinct().Count() <= 4).WithMessage("No more than 4 Replacement Produces are allowed")
                        .ForEach(y => y.ValidateProperty().GreaterThan(0));
                    });
                });
            }
        }
    }
}
