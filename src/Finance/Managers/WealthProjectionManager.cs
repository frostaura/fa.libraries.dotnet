using Finance.Interfaces.Managers;
using Finance.Models;
using Newtonsoft.Json;

namespace Finance.Managers
{
    /// <summary>
    /// A manager for wealth projection features.
    /// </summary>
    public class WealthProjectionManager : IWealthProjectionManager
	{
        /// <summary>
		/// Project to a specific date.
		/// </summary>
		/// <param name="request">Required projection request data.</param>
		/// <param name="targetDate">The target date to project to.</param>
		/// <returns>The projection data.</returns>
		public Task<ProjectionResponse> ProjectToDateAsync(ProjectionRequest request, DateTime targetDate)
        {
            var shouldProjectDelegate = new Func<ProjectionRequest, int, DateTime, bool>((req, monthIndex, currentDate) => currentDate <= targetDate);

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
            var shouldProjectDelegate = new Func<ProjectionRequest, int, DateTime, bool>((req, monthIndex, currentDate) => req
                                                                                                    .Accounts
                                                                                                    .SelectMany(a => a.Transactions)
                                                                                                    .Sum(t => t.Amount) >= targetNetWorth);

            return ProjectTillIsTerminalAsync(request, shouldProjectDelegate);
        }

        /// <summary>
        /// Event that occurs at the beginning of each month's projection.
        /// </summary>
        public event Action<ProjectionRequest, int, DateTime> OnNextMonth;

        /// <summary>
		/// Project till a provided delegate determines the projection is terminal.
		/// </summary>
		/// <param name="request">Required projection request data.</param>
		/// <param name="isTerminalDelegate">The determiner for when to stop the projection.</param>
		/// <returns>The projection data.</returns>
		public Task<ProjectionResponse> ProjectTillIsTerminalAsync(ProjectionRequest request, Func<ProjectionRequest, int, DateTime, bool> isTerminalDelegate)
        {
            var clonedRequest = CloneRequest(request);
            var accountsWithNegativeBalances = clonedRequest
                                                .Accounts
                                                .Where(a => a.Balance < 0);
            var mainAccount = clonedRequest
                .Accounts
                .Single(a => a.SalaryDepositAccount);
            var runningDate = clonedRequest.ProjectionStartDate;
            var monthIndex = 0;

            // While the projection is not terminal, proceed one month at a time.
            while (isTerminalDelegate(clonedRequest, monthIndex, runningDate))
            {
                if (OnNextMonth != default) OnNextMonth(clonedRequest, monthIndex, runningDate);

                var salaryIncomeAmount = clonedRequest
                    .Income
                    .Single(i => i.Name.Contains("salary", StringComparison.CurrentCultureIgnoreCase))
                    .Amount;
                var taxableIncomeAmount = clonedRequest
                    .Income
                    .Where(i => i.Taxable)
                    .Sum(i => i.Amount);

                foreach (var condition in clonedRequest.Conditions)
                {
                    if (!condition.Key(clonedRequest, monthIndex, runningDate)) continue;

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

                    clonedRequest
                        .Income
                        .Add(item);
                }

                // Calculate income per-cycle (month).
                var absoluteIncomeItems = clonedRequest
                    .Income
                    .Where(i => i.Type == Enums.PricedItemType.Absolute)
                    .Select(i => new PricedItem
                    {
                        Amount = i.Amount,
                        Name = i.Name,
                        TimeStamp = runningDate
                    });
                var salaryRatioIncomeItems = clonedRequest
                    .Income
                    .Where(i => i.Type == Enums.PricedItemType.SalaryRatio)
                    .Select(i => new PricedItem
                    {
                        Amount = i.Amount * salaryIncomeAmount,
                        Name = i.Name,
                        TimeStamp = runningDate
                    });
                var incomeItems = absoluteIncomeItems.Concat(salaryRatioIncomeItems);

                // Calculate expenses per-cycle (month).
                var absoluteExpenseItems = clonedRequest
                    .Expenses
                    .Where(e => e.Type == Enums.PricedItemType.Absolute)
                    .Select(i => new PricedItem { Amount = -i.Amount, Name = i.Name, TimeStamp = runningDate });
                var salaryRatioExpenseItems = clonedRequest
                    .Expenses
                    .Where(e => e.Type == Enums.PricedItemType.SalaryRatio)
                    .Select(i => new PricedItem { Amount = -i.Amount * salaryIncomeAmount, Name = i.Name, TimeStamp = runningDate });
                var expenseItems = absoluteExpenseItems.Concat(salaryRatioExpenseItems);
                var nonExpiredAccounts = clonedRequest
                    .Accounts
                    .Where(a => a.ExpirationDate == default || a.ExpirationDate > runningDate)
                    .ToList();

                // For each account, add a priced item for scheduled transactions.
                foreach (var account in nonExpiredAccounts)
                {
                    // When an account has been settled, ignore it.
                    if (account.Type == Enums.AccountType.StopAtZero &&
                        account.Balance >= 0) continue;

                    // Log the deposit amount as a negative to the main account.
                    var absoluteDepositAmounts = account
                        .ScheduledTransactions
                        .Where(t => t.Amount > 0 && t.Type == Enums.PricedItemType.Absolute)
                        .Sum(t => t.Amount);
                    var salaryRatioDepositAmounts = account
                        .ScheduledTransactions
                        .Where(t => t.Amount > 0 && t.Type == Enums.PricedItemType.SalaryRatio)
                        .Sum(t => t.Amount * salaryIncomeAmount);
                    var depositAmount = absoluteDepositAmounts + salaryRatioDepositAmounts;

                    mainAccount.Transactions.Add(new PricedItem
                    {
                        Amount = -depositAmount,
                        Name = $"{account.Name} Deposit",
                        TimeStamp = runningDate
                    });

                    // Log Priced Item for each scheduled transaction.
                    var absoluteScheduledTransactions = account
                        .ScheduledTransactions
                        .Where(t => t.Type == Enums.PricedItemType.Absolute)
                        .Select(t => new PricedItem
                        {
                            Amount = t.Amount,
                            Name = t.Name,
                            TimeStamp = runningDate
                        });
                    var salaryRatioScheduledTransactions = account
                        .ScheduledTransactions
                        .Where(t => t.Type == Enums.PricedItemType.SalaryRatio)
                        .Select(t => new PricedItem
                        {
                            Amount = t.Amount * salaryIncomeAmount,
                            Name = t.Name,
                            TimeStamp = runningDate
                        });
                    var allScheduledTransactions = absoluteScheduledTransactions
                                                    .Concat(salaryRatioScheduledTransactions);

                    var accountWasPreviouslyInDebt = account.Balance < 0;
                    account.Transactions.AddRange(allScheduledTransactions);

                    // Apply interest for each account.
                    var interestPayment = new PricedItem
                    {
                        Name = $"{account.Name} Interest",
                        TimeStamp = runningDate,
                        Amount = (account.InterestRate * account
                                                            .Transactions
                                                            .Sum(t => t.Amount)) / 12
                    };

                    account.Transactions.Add(interestPayment);

                    if (accountWasPreviouslyInDebt && account.Balance >= 0)
                    {
                        // We paid off an account!
                    }
                }

                // Track all transactions so a simple SUM can be done later.
                mainAccount.Transactions.AddRange(incomeItems);
                mainAccount.Transactions.AddRange(expenseItems);

                // Get a list of all the accounts that are in debt. This strategy of debt settlement is tackling highest interest rates first.
                var accountsInDebt = clonedRequest
                    .Accounts
                    .Where(a => a.Balance < 0)
                    .OrderByDescending(a => a.InterestRate)
                    .ToList();

                foreach (var accountInDebt in accountsInDebt)
                {
                    // If there is money left, let's first use it to pay off any debt.
                    if (mainAccount.Balance <= 0) break;
                    if (accountInDebt.Balance >= 0) continue;

                    var deposit = new PricedItem
                    {
                        Name = $"{accountInDebt.Name} Deposit",
                        TimeStamp = runningDate,
                        Amount = (mainAccount.Balance > accountInDebt.Balance * -1) ?
                                    accountInDebt.Balance * -1 :
                                    mainAccount.Balance
                    };

                    accountInDebt.Transactions.Add(deposit);
                    mainAccount.Transactions.Add(new PricedItem
                    {
                        Name = deposit.Name,
                        Amount = -deposit.Amount,
                        TimeStamp = deposit.TimeStamp,
                        Type = deposit.Type
                    });

                    if (accountInDebt.Balance >= 0)
                    {
                        // We paid off an account!
                    }
                }

                // If there is still cash left in the checking account, invest it automatically.
                if(mainAccount.Balance > 0)
                {
                    var deposit = new PricedItem
                    {
                        Name = $"Reinvestment Deposit",
                        TimeStamp = runningDate,
                        Amount = mainAccount.Balance
                    };

                    clonedRequest.Accounts.First(a => a.DefaultInvestmentAccount).Transactions.Add(deposit);
                    mainAccount.Transactions.Add(new PricedItem
                    {
                        Name = deposit.Name,
                        Amount = -deposit.Amount,
                        TimeStamp = deposit.TimeStamp,
                        Type = deposit.Type
                    });
                }

                // Clean up once-off income items.
                clonedRequest
                    .Income
                    .Where(i => i.OnceOff)
                    .ToList()
                    .ForEach( i => clonedRequest.Income.Remove(i));

                runningDate = runningDate.AddMonths(1);
                monthIndex++;
            }

            return Task.FromResult(new ProjectionResponse
            {
                ProjectionEndDate = clonedRequest
                                        .Accounts
                                        .SelectMany(a => a.Transactions)
                                        .Max(t => t.TimeStamp),
                NetWorth = clonedRequest
                            .Accounts
                            .SelectMany(a => a.Transactions)
                            .Sum(t => t.Amount),
                AugmentedRequest = clonedRequest
            });
        }

        /// <summary>
        /// Deep clone a request.
        /// </summary>
        /// <returns>Cloned request.</returns>
        private ProjectionRequest CloneRequest(ProjectionRequest request)
        {
            var response = new ProjectionRequest
            {
                Accounts = Clone(request.Accounts),
                Conditions = request.Conditions,
                Expenses = Clone(request.Expenses),
                Income = Clone(request.Income),
                ProjectionStartDate = request.ProjectionStartDate
            };

            return response;
        }

        /// <summary>
        /// Deep clone an object.
        /// </summary>
        /// <typeparam name="T">Object type.</typeparam>
        /// <param name="obj">Object instance.</param>
        /// <returns>Cloned object.</returns>
        private T Clone<T>(T obj)
        {
            return JsonConvert
                .DeserializeObject<T>(JsonConvert.SerializeObject(obj));
        }
    }
}
