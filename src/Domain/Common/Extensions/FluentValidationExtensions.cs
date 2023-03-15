using FluentValidation;

namespace Domain.Common.Extensions
{
    public static class FluentValidationExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> ValidateNotNullProperty<T, TProperty>(this IRuleBuilderInitial<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.Cascade(CascadeMode.Stop).NotNull().WithMessage($"{nameof(TProperty)} is required");
        }
        public static IRuleBuilderOptions<T, TProperty> ValidateProperty<T, TProperty>(this IRuleBuilderInitial<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.Cascade(CascadeMode.Stop).NotEmpty().NotNull().WithMessage($"{nameof(TProperty)} is required");
        }
        public static IRuleBuilderOptions<T, TProperty> ValidateProperty<T, TProperty>(this IRuleBuilderInitialCollection<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.Cascade(CascadeMode.Stop).NotEmpty().NotNull().WithMessage($"{nameof(TProperty)} is required");
        }
        public static IRuleBuilderOptions<T, TProperty> ValidateEnumProperty<T, TProperty>(this IRuleBuilderInitial<T, TProperty> ruleBuilder)
        {
            return ruleBuilder.Cascade(CascadeMode.Stop).ValidateProperty().IsInEnum().WithMessage($"{nameof(TProperty)} is invalid");
        }
    }
}
