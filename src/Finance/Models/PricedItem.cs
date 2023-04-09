using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Finance.Enums;

namespace Finance.Models
{
	/// <summary>
	/// An item containing basic pricing information.
	/// </summary>
	[DebuggerDisplay("{Name}: {Amount}")]
	public class PricedItem
	{
		/// <summary>
		/// The name of the item.
		/// </summary>
		[Required(AllowEmptyStrings = false, ErrorMessage="A valid name is required.")]
		public string Name { get; set; }
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
	}
}

