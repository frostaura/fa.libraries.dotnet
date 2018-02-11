using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FrostAura.Libraries.Core.Extensions.Validation
{
    /// <summary>
    /// Validation extensions for types of Object.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Throws an exception when an object is null.
        /// </summary>
        /// <param name="obj">Object instance to be validated.</param>
        /// <param name="paramName">Parameter name of the object to be validated.</param>
        /// <exception cref="ArgumentNullException">The exception type to be thrown.</exception>
        public static void ThrowIfNull(this object obj, string paramName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
        
        /// <summary>
        /// Throws an exception when an object is null else return the object as a casted type.
        /// </summary>
        /// <param name="obj">Object instance to be validated.</param>
        /// <param name="paramName">Parameter name of the object to be validated.</param>
        /// <typeparam name="T">The type the validation context should be casted to.</typeparam>
        /// <returns>Validation context for chainability.</returns>
        /// <exception cref="ArgumentNullException">The exception type to be thrown when the validation context is null.</exception>
        /// <exception cref="ArgumentException">The exception type when the validation context can't be casted to T.</exception>
        public static T ThrowIfNull<T>(this T obj, string paramName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(paramName);
            }

            return obj;
        }

        /// <summary>
        /// Perform nested validation on an object
        /// </summary>
        /// <param name="obj">The object instance to validate.</param>
        /// <param name="validationResults"></param>
        public static bool IsValid(this object obj, out List<ValidationResult> validationResults)
        {
            var innerValidationContext = new ValidationContext(obj);
            
            validationResults = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, innerValidationContext, validationResults, true);
        }

        /// <summary>
        /// Perform nested validation on an object
        /// </summary>
        /// <param name="obj">The object instance to validate.</param>
        /// <param name="validationResults"></param>
        public static bool IsValid(this object obj, out List<string> validationResults)
        {
            var innerValidationContext = new ValidationContext(obj);
            var results = new List<ValidationResult>();
            bool isValid = obj.IsValid(out validationResults);

            validationResults = results
                .Select(vr => vr.ErrorMessage)
                .ToList();
            
            return isValid;
        }

        /// <summary>
        /// Perform nested validation and throw a validation exception if any fail.
        /// </summary>
        /// <param name="obj">The object instance to validate.</param>
        /// <param name="paramName">Parameter name of root object.</param>
        /// <exception cref="ValidationException">Custom validation exception containing validation results.</exception>
        public static T ThrowIfInvalid<T>(this T obj, string paramName)
        {
            var validationResults = new List<ValidationResult>();
            bool isValid = obj.IsValid(out validationResults);
            
            if(!isValid) throw new FrostAura
                .Libraries
                .Core
                .Exceptions
                .Validation.ValidationException(paramName, validationResults);

            return obj;
        }
    }
}