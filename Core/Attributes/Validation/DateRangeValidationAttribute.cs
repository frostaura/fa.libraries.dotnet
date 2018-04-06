using System;
using System.ComponentModel.DataAnnotations;

namespace FrostAura.Libraries.Core.Attributes.Validation
{
    /// <summary>
    /// Custom validator for checking date ranges.
    /// </summary>
    public class DateRangeValidationAttribute : RangeAttribute
    {
        public DateRangeValidationAttribute(int daysBeforeToday = 0, int daysAfterToday = 0)
            :base(typeof(DateTime), DateTime.Today.AddDays(daysBeforeToday*-1).ToShortDateString(), DateTime.Today.AddDays(daysAfterToday).ToShortDateString())
        { }
    }
}
