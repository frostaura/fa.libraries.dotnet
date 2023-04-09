using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Finance.Enums;

namespace Finance.Models
{
	/// <summary>
	/// An item containing basic pricing information with a transaction date.
	/// </summary>
	[DebuggerDisplay("[{TransactionDate}]: {Name}: {Amount}")]
	public class PricedTransactionItem : PricedItem
	{
		/// <summary>
		/// The date the item transaction took place.
		/// </summary>
		public DateTime TransactionDate { get; set; }
	}
}

