using System;
using System.ComponentModel.DataAnnotations;

namespace FrostAura.Libraries.Core.Attributes.Validation
{
    /// <summary>
    /// Custom validator for URLs with mustache placeholders.
    /// </summary>
    public class UrlWithPlaceholderValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Overridden implementation of IsValid to support custom URL validation.
        /// </summary>
        /// <param name="value">URL to validate.</param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(!(value is string) || !IsUrlValid(value as string)) return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }

        /// <summary>
        /// Perform actual URL validation.
        /// </summary>
        /// <param name="url">URL to validate.</param>
        /// <returns>Whether or not the URL is valid.</returns>
        private bool IsUrlValid(string url)
        {
            // Ignore validation for any urls with placeholders
            if (url.Trim().StartsWith("{")) return true;
            if (!url.StartsWith("https") && !url.StartsWith("http")) url = "http://" + url;

            return Uri.IsWellFormedUriString(url.Replace("{", "").Replace("}", ""), UriKind.Absolute);
        }
    }
}
