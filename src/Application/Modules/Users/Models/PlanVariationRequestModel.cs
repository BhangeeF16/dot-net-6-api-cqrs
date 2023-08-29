using Domain.Common.Extensions;
using Domain.Entities.GeneralModule;
using FluentValidation;

namespace Application.Modules.Users.Models;

public class PlanVariationRequestModel
{
    public int fk_PlanID { get; set; }
    public bool IsOrganic { get; set; } = true;
    public ShippingFrequency Frequency { get; set; }
    public ItemsType ItemsType { get; set; }

    public class Validator : AbstractValidator<PlanVariationRequestModel?>
    {
        public Validator()
        {
            RuleFor(c => c.fk_PlanID).ValidateProperty();
            RuleFor(c => c.Frequency).ValidateEnumProperty();
            RuleFor(c => c.ItemsType).ValidateEnumProperty();
        }
    }
}
