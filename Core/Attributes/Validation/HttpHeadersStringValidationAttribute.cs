using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace FrostAura.Libraries.Core.Attributes.Validation
{
    /// <summary>
    /// Custom validator for HTTP headers collection string.
    /// </summary>
    public class HttpHeadersStringValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Overridden implementation of IsValid to support custom headers validation.
        /// </summary>
        /// <param name="value">URL to validate.</param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(!(value is string) || !AreHeadersValid(value as string)) return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }

        /// <summary>
        /// Perform actual header validation.
        /// </summary>
        /// <param name="headers">HTTP headers stringified to validate.</param>
        /// <returns>Whether or not the headers are valid.</returns>
        private bool AreHeadersValid(string headers)
        {
            if (string.IsNullOrWhiteSpace(headers) || headers.Replace(" ", "") == "[]") return true;

            try
            {
                var parsedHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(headers);

                foreach (KeyValuePair<string, string> parsedHeader in parsedHeaders)
                {
                    if (string.IsNullOrWhiteSpace(parsedHeader.Key) || string.IsNullOrWhiteSpace(parsedHeader.Value))
                        return false;
                }
            }
            catch (JsonReaderException)
            {
                return false;
            }

            return true;
        }
    }
}
