using System.Diagnostics;
using Finance.Enums;

namespace Finance.Models
{
	/// <summary>
	/// Financial account information.
	/// </summary>
	[DebuggerDisplay("{Name} ({InterestRate}):  R {Amount} -> R {RunningBalance} (R {Available} available)")]
	public class Account : PricedItem
    {
		/// <summary>
		/// The interest rate, positive or nagative, expressed as a ratio.
		///
		/// Example: 0.15 (15%)
		/// </summary>
		public double InterestRate { get; set; } = 0.0;
		/// <summary>
		/// Max value the account can be.
		/// </summary>
		public double Limit { get; set; }
		/// <summary>
		/// How much is still available on the account given all the transactions and the limit.
		/// </summary>
		public double Available => Math.Round(Transactions.Sum(t => t.Amount) + Limit, 2);
		/// <summary>
		/// Whether this is the account that the primary salary gets deposited into.
		/// </summary>
		public bool SalaryDepositAccount { get; set; }
		/// <summary>
		/// Transactions that are scheduled for the account.
		///
		/// Example: Account fees, interest and payments.
		/// </summary>
		public List<PricedItem> ScheduledTransactions = new List<PricedItem>();
		/// <summary>
		/// All transactions for the respective account.
		/// </summary>
		public List<PricedTransactionItem> Transactions = new List<PricedTransactionItem>();
		/// <summary>
		/// The running balance as determined from all active transactions for the account.
		/// </summary>
		public double RunningBalance => Math.Round(Transactions.Sum(t => t.Amount), 2);
		/// <summary>
		/// The type of the account.
		/// </summary>
		public AccountType Type { get; set; } = AccountType.Debit;
		/// <summary>
		/// The expiration date of the account, if any.
		/// </summary>
		public DateTime ExpirationDate { get; set; }
	}
}

