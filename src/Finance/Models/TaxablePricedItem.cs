using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Finance.Models
{
	/// <summary>
	/// An item containing basic pricing information with tax support.
	/// </summary>
	[DebuggerDisplay("{Name}: {Amount}")]
	public class TaxablePricedItem : PricedItem
	{
		/// <summary>
		/// Whether the item is taxable  like in the example of a salary.
		/// </summary>
		public bool Taxable { get; set; }
	}
}

