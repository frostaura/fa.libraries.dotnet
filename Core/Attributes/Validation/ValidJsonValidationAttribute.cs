using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Newtonsoft.Json;

namespace FrostAura.Libraries.Core.Attributes.Validation
{
    /// <summary>
    /// Custom validator for checking for valid JSON
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ValidJsonValidationAttribute : ValidationAttribute
    {
        public Type TypeValidationType { get; set; }

        /// <summary>
        /// Overridden implementation of IsValid to support custom validation
        /// </summary>
        /// <param name="value">URL to validate</param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string validationError = GetValidationError(value as string);

            if (string.IsNullOrEmpty(validationError)) return ValidationResult.Success;
            
            return new ValidationResult(validationError);
        }

        /// <summary>
        /// Check whether or not a string is valid JSON
        /// </summary>
        /// <param name="jsonString">String to check</param>
        /// <returns>Valid JSON</returns>
        private string GetValidationError(string jsonString)
        {
            if (string.IsNullOrWhiteSpace(jsonString)) return null;

            try
            {
                if (TypeValidationType == null) JsonConvert.DeserializeObject(jsonString);
                else
                {
                    var parsedJson = JsonConvert.DeserializeObject(jsonString, TypeValidationType);
                    var validationContext = new ValidationContext(parsedJson);
                    var validationResults = new List<ValidationResult>();
                    bool isObjectValid = Validator.TryValidateObject(parsedJson, validationContext, validationResults, true);

                    if (!isObjectValid)
                    {
                        return $"{ErrorMessage} > {validationResults.Select(vr => vr.ErrorMessage).Aggregate((a, b) => $"{a} | {b}")}";
                    }
                }
            }
            catch (JsonReaderException)
            {
                return ErrorMessage;
            }

            return null;
        }
    }
}
