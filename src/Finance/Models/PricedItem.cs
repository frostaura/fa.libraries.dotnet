using System.Diagnostics;
using Finance.Enums;
using FrostAura.Libraries.Data.Models.EntityFramework;

namespace Finance.Models
{
    /// <summary>
    /// An item containing basic pricing information.
    /// </summary>
    [DebuggerDisplay("{Name}: {Amount}")]
	public class PricedItem : BaseNamedEntity
    {
		/// <summary>
		/// The amount of the item. This can be positive or negative.
		/// </summary>
		public double Amount { get; set; }
		/// <summary>
		/// The type of the priced item.
		/// </summary>
		public PricedItemType Type { get; set; } = PricedItemType.Absolute;
		/// <summary>
		/// Whether this item is a once off item or a recurring one if not.
		/// </summary>
		public bool OnceOff { get; set; }
		/// <summary>
		/// Whether the priced item should be deducted from salary.
		/// </summary>
		public bool FromSalary { get; set; }
	}
}

