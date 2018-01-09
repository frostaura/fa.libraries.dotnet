using System.ComponentModel.DataAnnotations;
using FrostAura.Libraries.Core.Enums;

namespace FrostAura.Libraries.Core.Attributes.Validation
{
    /// <summary>
    /// Custom validator for doing generic comparisons
    /// Usage: {ValidatingValue} {ChosenOperator} {ComparisonValue}
    /// </summary>
    public class ComparisonValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Value that comparison will be performed on
        /// </summary>
        public object CompareToValue { get; set; }

        /// <summary>
        /// Operator to apply in comparison
        /// </summary>
        public ValidationOperator Operator { get; set; }

        /// <summary>
        /// Overridden implementation of IsValid to support custom URL validation
        /// </summary>
        /// <param name="value">URL to validate</param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return PerformComparison(value) ? ValidationResult.Success : new ValidationResult(ErrorMessage);
        }

        private bool PerformComparison(object value)
        {
            switch (Operator)
            {
                case ValidationOperator.Equals:
                    return value.Equals(CompareToValue);
                case ValidationOperator.NotEquals:
                    return !value.Equals(CompareToValue);
                default:
                    return false;
            }
        }
    }
}
