using System.Diagnostics;
using Finance.Enums;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Data.Models.EntityFramework;

namespace Finance.Models
{
	/// <summary>
	/// Financial account information.
	/// </summary>
	[DebuggerDisplay("{Name} ({InterestRate}): {Balance}")]
	public class Account : BaseNamedEntity
    {
		/// <summary>
		/// The interest rate, positive or nagative, expressed as a ratio.
		///
		/// Example: 0.15 (15%)
		/// </summary>
		public double InterestRate { get; set; } = 0.0;
		/// <summary>
		/// Whether this account is the default investment account.
		/// </summary>
		public bool DefaultInvestmentAccount { get; set; }
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
		public List<PricedItem> Transactions = new List<PricedItem>();
		/// <summary>
		/// The running balance as determined from all active transactions for the account.
		/// </summary>
		public double Balance => Math.Round(Transactions.Sum(t => t.Amount), 2);
		/// <summary>
		/// The type of the account.
		/// </summary>
		public AccountType Type { get; private set; }
		/// <summary>
		/// The expiration date of the account, if any.
		/// </summary>
		public DateTime ExpirationDate { get; set; }

		/// <summary>
		/// Overloaded constructor to allow for providing parameters.
		/// </summary>
		/// <param name="name">Name of the account.</param>
		/// <param name="balance">Initial balance of the account, if any.</param>
		/// <param name="accountType">Account type.</param>
		public Account(string name, double balance = 0.0, AccountType accountType = AccountType.Debit)
		{
			Name = name.ThrowIfNullOrWhitespace(nameof(name));
			Type = accountType;

			if (balance != 0.0)
			{
                Transactions.Add(new PricedItem
                {
                    Amount = balance,
                    Name = "Initial Balance Brought Forward"
                });
            }
        }
	}
}
