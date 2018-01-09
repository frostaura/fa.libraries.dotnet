using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace FrostAura.Libraries.Core.Extensions.Decoration
{
    /// <summary>
    /// Validation extensions for enumerations.
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Get the description value for an enum from the DescriptionAttribute, if any.
        /// </summary>
        /// <param name="enumValue">Enumeration value to get a description of.</param>
        /// <returns>The description from the DescriptionAttribute or null.</returns>
        public static string Description(this System.Enum enumValue)
        {
            MemberInfo memberInfo = enumValue
                .GetType()
                .GetMember(enumValue.ToString())
                .FirstOrDefault();

            if (memberInfo != null)
            {
                var descriptionAttribute = (DescriptionAttribute) memberInfo
                    .GetCustomAttributes(typeof(DescriptionAttribute))
                    .FirstOrDefault();

                if (descriptionAttribute != null)
                {
                    return descriptionAttribute.Description;
                }
            };

            return null;
        }

        /// <summary>
        /// Get the description value for an enum from the Description, if any and substitute placeholders.
        /// </summary>
        /// <param name="enumValue">Enumeration value to get a description of.</param>
        /// <param name="params">Placeholders to inject into description string.</param>
        /// <returns>The description from the DescriptionAttribute or null.</returns>
        public static string Description(this System.Enum enumValue, params string[] @params)
        {
            string description = enumValue
                .Description();

            if (string.IsNullOrWhiteSpace(description)) return null;

            return string.Format(description, @params);
        }
    }
}