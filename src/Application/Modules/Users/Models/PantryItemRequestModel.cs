using Domain.Common.Extensions;
using Domain.Entities.GeneralModule;
using FluentValidation;

namespace Application.Modules.Users.Models
{
    public class PantryItemRequestModel
    {
        public int ID { get; set; }
        public PantryItemFrequency Frequency { get; set; }
        public string? Notes { get; set; }
        public int Qty { get; set; }

        public class Validator : AbstractValidator<PantryItemRequestModel?>
        {
            public Validator()
            {
                RuleFor(c => c.ID).ValidateProperty();
                RuleFor(c => c.Qty).ValidateProperty();
                RuleFor(c => c.Frequency).ValidateEnumProperty();
            }
        }
    }
}
