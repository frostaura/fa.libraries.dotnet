using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FrostAura.Libraries.Core.Attributes.Validation
{
    /// <summary>
    /// Custom validator for checking for valid collection
    /// </summary>
    public class CollectionValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Whether or not empty collection is allowed
        /// </summary>
        public bool AllowEmptyCollection { get; set; } = false;

        /// <summary>
        /// Whether or not empty collection is allowed
        /// </summary>
        public bool ValidateCollectionEntries { get; set; } = true;

        /// <summary>
        /// Minimum count reuired in collection to pass validation
        /// </summary>
        public int MinCount { get; set; } = 1;

        /// <summary>
        /// Overridden implementation of IsValid to support custom validation
        /// </summary>
        /// <param name="value">URL to validate</param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string validationError = GetValidationError(value as IEnumerable<object>);

            if(string.IsNullOrEmpty(validationError)) return ValidationResult.Success;

            return new ValidationResult(validationError);
        }

        /// <summary>
        /// Check whether or not the collection is valid
        /// </summary>
        /// <param name="collection">String to check</param>
        /// <returns>Valid JSON</returns>
        private string GetValidationError(IEnumerable<object> collection)
        {
            if (collection == null) return ErrorMessage;
            if (!AllowEmptyCollection && collection.Count() < MinCount) return ErrorMessage;
            if (ValidateCollectionEntries && collection.Any())
            {
                // Validate all collection entries
                foreach (object o in collection)
                {
                    var innerValidationContext = new ValidationContext(o);
                    var validationResults = new List<ValidationResult>();

                    // Upon failure of the first collection item, fail the validation
                    if (!Validator.TryValidateObject(o, innerValidationContext, validationResults, true))
                    {
                        return $"{ErrorMessage} > {validationResults.Select(vr => vr.ErrorMessage).Aggregate((a, b) => $"{a} | {b}")}";
                    }
                }
            }

            return null;
        }
    }
}
