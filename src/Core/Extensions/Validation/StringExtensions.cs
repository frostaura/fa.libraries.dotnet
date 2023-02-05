using System;

namespace FrostAura.Libraries.Core.Extensions.Validation
{
    /// <summary>
    /// Validation extensions for types of string.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Throws an exception when a string is null or empty.
        /// </summary>
        /// <param name="str">String instance to be validated.</param>
        /// <param name="paramName">Parameter name of the string to be validated.</param>
        /// <exception cref="ArgumentNullException">The exception type to be thrown.</exception>
        /// <returns>Validation context for chainability.</returns>
        public static string ThrowIfNullOrWhitespace(this string str, string paramName)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentNullException(paramName);
            }

            return str;
        }
    }
}