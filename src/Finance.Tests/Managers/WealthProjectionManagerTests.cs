using Finance.Enums;
using Finance.Managers;
using Finance.Models;
using Xunit;

namespace Finance.Tests.Managers
{
    public class WealthProjectionManagerTests
	{
        [Fact]
        public void Construction_WithValidParams_ShouldConstruct()
        {
            var actual = new WealthProjectionManager();

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task ProjectToDateAsync_WithValidData_ShouldProject()
        {
            // Setup
            var instance = new WealthProjectionManager();
            var request = new ProjectionRequest
            {
                Accounts = new List<Account>
                {

                    new Account("Alexander Forbes", balance: 755947)
                    {
                        InterestRate = 0.1, // Growth percentage yearly.
                        ScheduledTransactions = new List<PricedItem>
                        {
                            // The double amount being defined and subtracted from the salary account, is offset by an income item.
                            new PricedItem{ Name = "AF Provident Fund Admin Fee", Amount = -500 },
                            new PricedItem{ Name = "AF Provident Fund (Employee Deposit)", Amount = 0.05, Type = PricedItemType.SalaryRatio, FromSalary = true },
                            new PricedItem{ Name = "AF Provident Fund (Company Deposit)", Amount = 0.05, Type = PricedItemType.SalaryRatio }
                        }
                    },
                    new Account("Securities", balance: 17932)
                    {
                        InterestRate = 0.07, // Conservative percentage.
                        DefaultInvestmentAccount = true,
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "SPY", Amount = 1000, FromSalary = true }
                        }
                    },
                    new Account("Crypto", balance: 42889)
                    {
                        InterestRate = 0.1, // Growth percentage yearly.
                        DefaultInvestmentAccount = true,
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "BTC", Amount = 1000, FromSalary = true }
                        }
                    },
                    new Account("MTN Mobile (iPhone 14 Pro Max)", accountType: AccountType.Repeat)
                    {
                        ExpirationDate = new DateTime(2024, 10, 31),
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "Payment", Amount = 1500, FromSalary = true }
                        }
                    },
                    new Account("MTN Mobile (iPhone 15 Pro Max)", accountType: AccountType.Repeat)
                    {
                        ExpirationDate = new DateTime(2026, 12, 31),
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "Payment", Amount = 1500, FromSalary = true }
                        }
                    },
                    new Account("Apple Watch (Discovery)", accountType: AccountType.Repeat)
                    {
                        ExpirationDate = new DateTime(2024, 12, 31),
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "Payment", Amount = 450, FromSalary = true }
                        }
                    },
                    new Account("MFC Vehicle Finance", balance: -406831, accountType: AccountType.StopAtZero)
                    {
                        InterestRate = 0.1465,
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "Payment", Amount = 11400, FromSalary = true },
                            new PricedItem{ Name = "Service Fee", Amount = -60 },
                            new PricedItem{ Name = "Vat", Amount = -9 }
                        }
                    },
                    new Account("FNB Fusion")
                    {
                        SalaryDepositAccount = true,
                        InterestRate = 0.2225,
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "Bank Account Charges", Amount = -650 }
                        }
                    },
                    new Account("FNB Fusion (Overdraft)", balance: -35776, accountType: AccountType.StopAtZero)
                    {
                        InterestRate = 0.2225
                    },
                    new Account("FNB Credit", balance: -290704, accountType: AccountType.StopAtZero)
                    {
                        Name = "FNB Credit",
                        InterestRate = 0.1975
                    },
                    new Account("SA Home Loan", balance: -1735562, accountType: AccountType.StopAtZero)
                    {
                        InterestRate = 0.119,
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "Payment", Amount = 19500, FromSalary = true },
                            new PricedItem{ Name = "Bank Account Charges", Amount = -70 },
                        }
                    }
                },
                Income = new List<TaxablePricedItem>
                {
                    new TaxablePricedItem{ Name = "Basic Salary", Amount = 151018, Taxable = true },
                    new TaxablePricedItem{ Name = "Medical Aid Allowance", Amount = 2000 },
                    new TaxablePricedItem{ Name = "AF Provident Fund", Amount = 0.05, Type = PricedItemType.SalaryRatio },
                    new TaxablePricedItem{ Name = "AF Provident Fund Admin Fee", Amount = 173 },
                    new TaxablePricedItem{ Name = "Funeral Cover", Amount = 3 },
                    new TaxablePricedItem{ Name = "Connectivity Allowance", Amount = 283 },
                    new TaxablePricedItem{ Name = "Mobile Phone Allowance", Amount = 350 }
                },
                Expenses = new List<PricedItem>
                {
                    new PricedItem{ Name = "Pay as you Earn", Amount = 0.3275, Type = PricedItemType.SalaryRatio }, // Conservative tax rate to compensate for bonuses. TODO: Calculate tax automatically based on a rules system. 
                    new PricedItem{ Name = "Unemployment Insurance Fund", Amount = 200 },
                    new PricedItem{ Name = "Gap Cover", Amount = 350 },
                    new PricedItem{ Name = "Canteen", Amount = 300 },
                    new PricedItem{ Name = "Discovery Medical Aid", Amount = 2500 },
                    new PricedItem{ Name = "Discovery Vitality", Amount = 350 },
                    new PricedItem{ Name = "Cool Ideas Fibre", Amount = 1500 },
                    new PricedItem{ Name = "Santam Insurance", Amount = 2450 },
                    new PricedItem{ Name = "Maid", Amount = 350 * 5 },
                    new PricedItem{ Name = "Fuel & Carwash", Amount = 2000 },
                    new PricedItem{ Name = "Groceries", Amount = 4000 },
                    new PricedItem{ Name = "Apple One", Amount = 250 },
                    new PricedItem{ Name = "Home Levies", Amount = 1700 },
                    new PricedItem{ Name = "Rates, Taxes & Electricity", Amount = 5500 },
                    new PricedItem{ Name = "Pet Supplies", Amount = 1500 },
                    new PricedItem{ Name = "Entertainment", Amount = 4000 },
                    new PricedItem{ Name = "YouTube Premium", Amount = 100 },
                    new PricedItem{ Name = "Infuse Pro", Amount = 50 },
                    new PricedItem{ Name = "Manscaping", Amount = 1000 },
                    new PricedItem{ Name = "Diving Savings", Amount = 2000 }
                }
            };

            // Increase salary each time it's increase month.
            instance.OnNextMonth += (monthRequest, monthIndex, runningDate) =>
            {
                if (runningDate.Month != 6) return;

                var salaryIncomeAmount = monthRequest
                    .Income
                    .Single(i => i.Name.Contains("salary", StringComparison.CurrentCultureIgnoreCase));

                Console.WriteLine("Increasing salary by 5% conservatively.");

                salaryIncomeAmount.Amount *= 1.05;
            };

            // Special once-off salary increase.
            instance.OnNextMonth += (monthRequest, monthIndex, runningDate) =>
            {
                if (runningDate.Month != 1 || runningDate.Year != 2024) return;

                var salaryIncomeAmount = monthRequest
                    .Income
                    .Single(i => i.Name.Contains("salary", StringComparison.CurrentCultureIgnoreCase));

                Console.WriteLine("Increasing salary by 10% (Special Increase).");

                salaryIncomeAmount.Amount *= 1.1;
            };

            // Register custom conditional transactions.
            request.Conditions[(req, monthIndex, monthDate) =>
            {
                var bonusMonths = new[] { 7, 12 };

                return bonusMonths.Contains(monthDate.Month);
            }] = new TaxablePricedItem
            {
                Name = "Bonus",
                Amount = 0.7,
                Taxable = true,
                OnceOff = true,
                Type = PricedItemType.SalaryRatio
            };
            request.Conditions[(req, monthIndex, monthDate) =>
            {
                var leaveEncashmentMonths = new[] { 8 };

                return leaveEncashmentMonths.Contains(monthDate.Month);
            }] = new TaxablePricedItem
            {
                Name = "Leave Encashment",
                Amount = 1.0 / 21 * 5, // Assuming 21 work days per month and encashing 5 days.
                Taxable = true,
                OnceOff = true,
                Type = PricedItemType.SalaryRatio
            };

            // Once-off expense(s)
            request.Conditions[(req, monthIndex, monthDate) =>
            {
                // Specifically for this month.
                var now = DateTime.Now;

                return monthDate.Month == now.Month && monthDate.Year == now.Year;
            }] = new TaxablePricedItem
            {
                Name = "Discovery Credit Settlement (VET Bill)",
                Amount = -13000,
                OnceOff = true,
                Type = PricedItemType.Absolute
            };

            // End conditions.
            var hasUnsettledAccountsTerminalCondition = new Func<ProjectionRequest, int, DateTime, bool>((req, monthIndex, monthDate) =>
            {
                var areAllAccountsSettled = req
                                                .Accounts
                                                .All(a => a.Balance >= 0);

                return !areAllAccountsSettled;
            });
            var hasNotMetFinancialGoalsYetTerminalCondition = new Func<ProjectionRequest, int, DateTime, bool>((req, monthIndex, monthDate) =>
            {
                var hasFinancialGoalsBeenMet = req
                                                .Accounts
                                                .Sum(a => a.Balance) >= 30000000;

                return !hasFinancialGoalsBeenMet;
            });
            var belowZeroNetWorthTerminalCondition = new Func<ProjectionRequest, int, DateTime, bool>((req, monthIndex, monthDate) =>
            {
                var isNetWorthAboveZero = req
                                                .Accounts
                                                .SelectMany(a => a.Transactions)
                                                .Sum(t => t.Amount) >= 0;

                return !isNetWorthAboveZero;
            });

            // Set projection start date.
            request.ProjectionStartDate = new DateTime(2024, 8, 28);

            var projectionForSpecificDate = await instance.ProjectToDateAsync(request, new DateTime(2024, 12, 31));

            // Feb 2026 (Prev: Oct 2025)
            var projectionForWhenNetWorthExceedsZero = await instance.ProjectTillIsTerminalAsync(request, belowZeroNetWorthTerminalCondition);

            // Aug 2027 (Prev: Jul 2027)
            var projectionForAllDebtSettled = await instance.ProjectTillIsTerminalAsync(request, hasUnsettledAccountsTerminalCondition);

            // July 2036 (Prev: June 2036)
            var projectionTillFinancialGoalsMet = await instance.ProjectTillIsTerminalAsync(request, hasNotMetFinancialGoalsYetTerminalCondition);

            // 11m (Prev: 11.5m)
            var projectTillRetirementAt40 = await instance.ProjectToDateAsync(request, new DateTime(2023 + (40 - 32), 09, 05));

            // 66.2m (Prev: 67.1m)
            var projectTillRetirementAt50 = await instance.ProjectToDateAsync(request, new DateTime(2023 + (50 - 32), 09, 05));

            // Aug 2025 (Prev: May 2025)
            var projectTillCarIsPaidOff = await instance.ProjectTillIsTerminalAsync(request, new Func<ProjectionRequest, int, DateTime, bool>((req, monthIndex, monthDate) =>
            {
                var accSettled = req
                    .Accounts
                    .First(a => a.Name == "MFC Vehicle Finance");

                return accSettled.Balance < 0;
            }));

            // Aug 2027 (Prev: Jul 2027)
            var projectTillHouseIsPaidOff = await instance.ProjectTillIsTerminalAsync(request, new Func<ProjectionRequest, int, DateTime, bool>((req, monthIndex, monthDate) =>
            {
                var accSettled = req
                    .Accounts
                    .First(a => a.Name == "SA Home Loan");

                return accSettled.Balance < 0;
            }));

            Assert.NotNull(projectionTillFinancialGoalsMet);
        }
    }
}
