using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FrostAura.Libraries.Core.Exceptions.Validation
{
    /// <summary>
    /// Custom validation exception.
    /// </summary>
    public class ValidationException : Exception
    {
        /// <summary>
        /// Collection of all validation errors that occured.
        /// </summary>
        public IEnumerable<ValidationResult> ValidationResults { get; }
        
        /// <summary>
        /// Overloaded constructor to allow for passing validation errors.
        /// </summary>
        /// <param name="paramName">Name of the root validated object.</param>
        /// <param name="validationResults">Collection of all validation errors that occured.</param>
        public ValidationException(string paramName, IEnumerable<ValidationResult> validationResults)
            :base($"Validation failed on root parameter '{paramName}'")
        {
            ValidationResults = validationResults;
        }
    }
}