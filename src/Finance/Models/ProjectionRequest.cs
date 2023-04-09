using Finance.Enums;

namespace Finance.Models
{
	/// <summary>
	/// All information required for making a projection.
	/// </summary>
	public class ProjectionRequest
	{
		/// <summary>
		/// All conditional transactions that should occur.
		/// </summary>
		public Dictionary<Func<int, DateTime, bool>, TaxablePricedItem> Conditions { get; set; } = new Dictionary<Func<int, DateTime, bool>, TaxablePricedItem>();
		/// <summary>
		/// All accounts information.
		/// </summary>
		public List<Account> Accounts { get; set; } = new List<Account>();
		/// <summary>
		/// All income items.
		/// </summary>
		public List<TaxablePricedItem> Income { get; set; } = new List<TaxablePricedItem>();
		/// <summary>
		/// All expense items.
		/// </summary>
		public List<PricedItem> Expenses { get; set; } = new List<PricedItem>();
    }
}

