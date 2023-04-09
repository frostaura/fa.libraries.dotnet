using Finance.Interfaces.Managers;
using Finance.Models;

namespace Finance.Managers
{
    /// <summary>
    /// A manager for wealth projection features.
    /// </summary>
    public class WealthProjectionManager : IProjectionManager
	{
        /// <summary>
		/// Project to a specific date.
		/// </summary>
		/// <param name="request">Required projection request data.</param>
		/// <param name="targetDate">The target date to project to.</param>
		/// <returns>The projection data.</returns>
		public Task<ProjectionResponse> ProjectToDateAsync(ProjectionRequest request, DateTime targetDate)
        {
            var shouldProjectDelegate = new Func<int, DateTime, bool>((monthIndex, currentDate) => currentDate <= targetDate);

            return ProjectTillIsTerminalAsync(request, shouldProjectDelegate);
        }

        /// <summary>
        /// Project to a specific net worth. For example until a balance of 100 000 has been achieved.
        /// </summary>
        /// <param name="request">Required projection request data.</param>
        /// <param name="targetNetWorth">The target net worth to project to.</param>
        /// <returns>The projection data.</returns>
        public Task<ProjectionResponse> ProjectToNetWorthAsync(ProjectionRequest request, double targetNetWorth)
        {
            var shouldProjectDelegate = new Func<int, DateTime, bool>((monthIndex, currentDate) => request
                                                                                                    .Accounts
                                                                                                    .SelectMany(a => a.Transactions)
                                                                                                    .Sum(t => t.Amount) >= targetNetWorth);

            return ProjectTillIsTerminalAsync(request, shouldProjectDelegate);
        }

        /// <summary>
		/// Project till a provided delegate determines the projection is terminal.
		/// </summary>
		/// <param name="request">Required projection request data.</param>
		/// <param name="isTerminalDelegate">The determiner for when to stop the projection.</param>
		/// <returns>The projection data.</returns>
		public Task<ProjectionResponse> ProjectTillIsTerminalAsync(ProjectionRequest request, Func<int, DateTime, bool> isTerminalDelegate)
        {
            var accountsWithNegativeBalances = request
                                                .Accounts
                                                .Where(a => a.Amount < 0);
            var mainAccount = request
                .Accounts
                .Single(a => a.SalaryDepositAccount);
            // Create the monthly loop from the 1st of this month.
            var runningDate = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var monthIndex = 0;

            // Reset all account transactions.
            foreach (var account in request.Accounts)
            {
                account.Transactions.Clear();
                account.Transactions.Add(new PricedTransactionItem
                {
                    Amount = account.Amount,
                    Name = "Balance Brought Forward",
                    TransactionDate = runningDate
                });
            }

            while (isTerminalDelegate(monthIndex, runningDate))
            {
                var taxableIncomeAmount = request
                    .Income
                    .Where(i => i.Taxable)
                    .Sum(i => i.Amount);

                foreach (var condition in request.Conditions)
                {
                    if (!condition.Key(monthIndex, runningDate)) continue;

                    var item = new TaxablePricedItem
                    {
                        Amount = condition.Value.Amount,
                        Name = condition.Value.Name,
                        OnceOff = condition.Value.OnceOff,
                        Taxable = condition.Value.Taxable,
                        Type = condition.Value.Type
                    };

                    // Convert any ratios to absolutes.
                    if (item.Type == Enums.PricedItemType.SalaryRatio)
                    {
                        item.Amount = item.Amount * taxableIncomeAmount;
                        item.Type = Enums.PricedItemType.Absolute;
                    }

                    if(item.Taxable) taxableIncomeAmount += item.Amount;

                    request
                        .Income
                        .Add(item);
                }

                // Calculate income per-cycle (month).
                var absoluteIncomeItems = request
                    .Income
                    .Where(i => i.Type == Enums.PricedItemType.Absolute)
                    .Select(i => new PricedTransactionItem
                    {
                        Amount = i.Amount,
                        Name = i.Name,
                        TransactionDate = runningDate
                    });
                var salaryRatioIncomeItems = request
                    .Income
                    .Where(i => i.Type == Enums.PricedItemType.SalaryRatio)
                    .Select(i => new PricedTransactionItem
                    {
                        Amount = i.Amount * taxableIncomeAmount,
                        Name = i.Name,
                        TransactionDate = runningDate
                    });
                var incomeItems = absoluteIncomeItems.Concat(salaryRatioIncomeItems);
                // Calculate expenses per-cycle (month).
                var absoluteExpenseItems = request
                    .Expenses
                    .Where(e => e.Type == Enums.PricedItemType.Absolute)
                    .Select(i => new PricedTransactionItem { Amount = -i.Amount, Name = i.Name, TransactionDate = runningDate });
                var salaryRatioExpenseItems = request
                    .Expenses
                    .Where(e => e.Type == Enums.PricedItemType.SalaryRatio)
                    .Select(i => new PricedTransactionItem { Amount = -i.Amount * taxableIncomeAmount, Name = i.Name, TransactionDate = runningDate });
                var expenseItems = absoluteExpenseItems.Concat(salaryRatioExpenseItems);
                var nonExpiredAccounts = request
                    .Accounts
                    .Where(a => a.ExpirationDate == default || a.ExpirationDate > runningDate)
                    .ToList();

                // For each account, add a priced item for scheduled transactions.
                foreach (var account in nonExpiredAccounts)
                {
                    // When an account has been settled, ignore it.
                    if (account.Type == Enums.AccountType.StopAtZero &&
                        account.RunningBalance >= 0) continue;

                    // Log the deposit amount as a negative to the main account.
                    var absoluteDepositAmounts = account
                        .ScheduledTransactions
                        .Where(t => t.Amount > 0 && t.Type == Enums.PricedItemType.Absolute)
                        .Sum(t => t.Amount);
                    var salaryRatioDepositAmounts = account
                        .ScheduledTransactions
                        .Where(t => t.Amount > 0 && t.Type == Enums.PricedItemType.SalaryRatio)
                        .Sum(t => t.Amount * taxableIncomeAmount);
                    var depositAmount = absoluteDepositAmounts + salaryRatioDepositAmounts;

                    mainAccount.Transactions.Add(new PricedTransactionItem
                    {
                        Amount = -depositAmount,
                        Name = $"{account.Name} Deposit",
                        TransactionDate = runningDate
                    });

                    // Log Priced Item for each scheduled transaction.
                    var absoluteScheduledTransactions = account
                        .ScheduledTransactions
                        .Where(t => t.Type == Enums.PricedItemType.Absolute)
                        .Select(t => new PricedTransactionItem
                        {
                            Amount = t.Amount,
                            Name = t.Name,
                            TransactionDate = runningDate
                        });
                    var salaryRatioScheduledTransactions = account
                        .ScheduledTransactions
                        .Where(t => t.Type == Enums.PricedItemType.SalaryRatio)
                        .Select(t => new PricedTransactionItem
                        {
                            Amount = t.Amount * taxableIncomeAmount,
                            Name = t.Name,
                            TransactionDate = runningDate
                        });
                    var allScheduledTransactions = absoluteScheduledTransactions
                                                    .Concat(salaryRatioScheduledTransactions);

                    var accountWasPreviouslyInDebt = account.RunningBalance < 0;
                    account.Transactions.AddRange(allScheduledTransactions);

                    // Apply interest for each account.
                    var interestPayment = new PricedTransactionItem
                    {
                        Name = $"{account.Name} Interest",
                        TransactionDate = runningDate,
                        Amount = (account.InterestRate * account
                                                            .Transactions
                                                            .Sum(t => t.Amount)) / 12
                    };

                    account.Transactions.Add(interestPayment);

                    if (accountWasPreviouslyInDebt && account.RunningBalance >= 0)
                    {
                        // We paid off an account!
                    }
                }

                // Track all transactions so a simple SUM can be done later.
                mainAccount.Transactions.AddRange(incomeItems);
                mainAccount.Transactions.AddRange(expenseItems);

                // Get a list of all the accounts that are in debt. This strategy of debt settlement is tackling highest interest rates first.
                var accountsInDebt = request
                    .Accounts
                    .Where(a => a.RunningBalance < 0)
                    .OrderByDescending(a => a.InterestRate)
                    .ToList();

                foreach (var accountInDebt in accountsInDebt)
                {
                    // If there is money left, let's first use it to pay off any debt.
                    if (mainAccount.Available <= 0) break;
                    if (accountInDebt.RunningBalance >= 0) continue;

                    var deposit = new PricedTransactionItem
                    {
                        Name = $"{accountInDebt.Name} Deposit",
                        TransactionDate = runningDate,
                        Amount = (mainAccount.Available > accountInDebt.RunningBalance * -1) ?
                                    accountInDebt.RunningBalance * -1 :
                                    mainAccount.Available
                    };

                    accountInDebt.Transactions.Add(deposit);
                    mainAccount.Transactions.Add(new PricedTransactionItem
                    {
                        Name = deposit.Name,
                        Amount = -deposit.Amount,
                        TransactionDate = deposit.TransactionDate,
                        Type = deposit.Type
                    });

                    if (accountInDebt.RunningBalance >= 0)
                    {
                        // We paid off an account!
                    }
                }

                // Clean up once-off income items.
                request
                    .Income
                    .Where(i => i.OnceOff)
                    .ToList()
                    .ForEach( i => request.Income.Remove(i));

                runningDate = runningDate.AddMonths(1);
                monthIndex++;
            }

            return Task.FromResult(new ProjectionResponse
            {
                ProjectionEndDate = request
                                        .Accounts
                                        .SelectMany(a => a.Transactions)
                                        .Max(t => t.TransactionDate),
                NetWorth = request
                            .Accounts
                            .SelectMany(a => a.Transactions)
                            .Sum(t => t.Amount)
            });
        }
    }
}
