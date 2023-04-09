using System;
using Finance.Enums;
using Finance.Managers;
using Finance.Models;
using Xunit;

namespace Finance.Tests.Managers
{
	public class WealthProjectionManagerTests
	{
        [Fact]
        public async Task ProjectToDateAsync_WithValidData_ShouldProject()
        {
            // Setup
            var instance = new WealthProjectionManager();
            var isBonusMonthCondition = new Func<int, DateTime, bool>((monthIndex, monthDate) =>
            {
                var bonusMonths = new[] { 7, 12 };

                return bonusMonths.Contains(monthDate.Month);
            });
            var isLeaveEncashmentMonth = new Func<int, DateTime, bool>((monthIndex, monthDate) =>
            {
                var leaveEncashmentMonths = new[] { 1 };

                return leaveEncashmentMonths.Contains(monthDate.Month);
            });
            var isIncreaseMonth = new Func<int, DateTime, bool>((monthIndex, monthDate) => monthDate.Month == 6);
            var request = new ProjectionRequest
            {
                Accounts = new List<Account>
                {
                    new Account
                    {
                        Name = "Vitality Savings",
                        Amount = 0,
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "General Savings", Amount = 2000 }
                        }
                    },
                    new Account
                    {
                        Name = "Alexander Forbes",
                        Amount = 473749,
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "AF Provident Fund Admin Fee", Amount = -173 },
                            new PricedItem{ Name = "AF Provident Fund (Employee Deposit)", Amount = 0.05, Type = PricedItemType.SalaryRatio },
                            new PricedItem{ Name = "AF Provident Fund (Company Deposit)", Amount = 0.05, Type = PricedItemType.SalaryRatio }
                        }
                    },
                    new Account
                    {
                        Name = "Easy Equities",
                        Amount = 1000,
                        InterestRate = 0.1,
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "Deposit", Amount = 1500 }
                        }
                    },
                    new Account
                    {
                        Name = "MTN Mobile",
                        Type = AccountType.Repeat,
                        Limit = 0,
                        ExpirationDate = new DateTime(2024, 12, 31),
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "Payment", Amount = 1550 }
                        }
                    },
                    new Account
                    {
                        Name = "FNB Devices (TV)",
                        Type = AccountType.Repeat,
                        Limit = 0,
                        ExpirationDate = new DateTime(2024, 12, 31),
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "Payment", Amount = 1150 }
                        }
                    },
                    new Account
                    {
                        Name = "MFC Vehicle Finance",
                        Amount = -580723,
                        InterestRate = 0.134,
                        Type = AccountType.StopAtZero,
                        Limit = 0,
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "Payment", Amount = 11100 },
                            new PricedItem{ Name = "Service Fee", Amount = -60 },
                            new PricedItem{ Name = "Vat", Amount = -9 }
                        }
                    },
                    new Account
                    {
                        Name = "Mobicred",
                        Amount = -19389,
                        Limit = 20000,
                        InterestRate = 0.22,
                        Type = AccountType.StopAtZero,
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "Payment", Amount = 1500 }
                        }
                    },
                    new Account
                    {
                        Name = "FNB Fusion",
                        Amount = -61560,
                        Limit = 65000,
                        SalaryDepositAccount = true,
                        InterestRate = 0.2125,
                        Type = AccountType.StopAtZero,
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "Bank Account Charges", Amount = -650 }
                        }
                    },
                    new Account
                    {
                        Name = "FNB Credit",
                        Amount = -170008,
                        Limit = 170000,
                        InterestRate = 0.2125,
                        Type = AccountType.StopAtZero,
                    },
                    new Account
                    {
                        Name = "Discovery Credit",
                        Amount = -49740,
                        Limit = 50000,
                        InterestRate = 0.21,
                        Type = AccountType.StopAtZero,
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "Bank Account Charges", Amount = -150 }
                        }
                    },
                    new Account
                    {
                        Name = "FNB Revolving Loan",
                        Amount = -35028,
                        InterestRate = 0.1825,
                        Type = AccountType.StopAtZero,
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "Payment", Amount = 1050 },
                            new PricedItem{ Name = "Bank Account Charges", Amount = -70 }
                        }
                    },
                    new Account
                    {
                        Name = "FNB Personal Loan (22.15% - 1.4K)",
                        Amount = -15707,
                        InterestRate = 0.2215,
                        Type = AccountType.StopAtZero,
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "Payment", Amount = 1450 },
                            new PricedItem{ Name = "Bank Account Charges", Amount = -70 },
                            new PricedItem{ Name = "Credit Protection", Amount = -140 }
                        }
                    },
                    new Account
                    {
                        Name = "FNB Personal Loan (20.65% - 2.35K)",
                        Amount = -56256,
                        InterestRate = 0.2065,
                        Type = AccountType.StopAtZero,
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "Payment", Amount = 2400 },
                            new PricedItem{ Name = "Bank Account Charges", Amount = -70 },
                            new PricedItem{ Name = "Credit Protection", Amount = -240 }
                        }
                    },
                    new Account
                    {
                        Name = "FNB Personal Loan (23.11% - 1.7K)",
                        Amount = -47207,
                        InterestRate = 0.2311,
                        Type = AccountType.StopAtZero,
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "Payment", Amount = 1700 },
                            new PricedItem{ Name = "Bank Account Charges", Amount = -70 },
                            new PricedItem{ Name = "Credit Protection", Amount = -240 }
                        }
                    },
                    new Account
                    {
                        Name = "SA Home Loan",
                        Amount = -1571752,
                        InterestRate = 0.105,
                        Type = AccountType.StopAtZero,
                        ScheduledTransactions = new List<PricedItem>
                        {
                            new PricedItem{ Name = "Payment", Amount = 16290 },
                            new PricedItem{ Name = "Bank Account Charges", Amount = -70 },
                        }
                    }
                },
                Income = new List<TaxablePricedItem>
                {
                    new TaxablePricedItem{ Name = "Basic Salary", Amount = 127225, Taxable = true },
                    new TaxablePricedItem{ Name = "Medical Aid Subsidy", Amount = 2000 },
                    new TaxablePricedItem{ Name = "Life Cover", Amount = 522 },
                    new TaxablePricedItem{ Name = "Disability", Amount = 45 },
                    new TaxablePricedItem{ Name = "Income Protector", Amount = 590 },
                    new TaxablePricedItem{ Name = "AF Provident Fund", Amount = 0.05, Type = PricedItemType.SalaryRatio },
                    new TaxablePricedItem{ Name = "AF Provident Fund Admin Fee", Amount = 173 },
                    new TaxablePricedItem{ Name = "Funeral Cover", Amount = 3 },
                    new TaxablePricedItem{ Name = "Connectivity Allowance", Amount = 283 },
                    new TaxablePricedItem{ Name = "Cellphone Re-imbursement", Amount = 350 }
                },
                Expenses = new List<PricedItem>
                {
                    new PricedItem{ Name = "Pay as you Earn", Amount = 0.33, Type = PricedItemType.SalaryRatio }, // Auto-add?
                    new PricedItem{ Name = "Unemployment Insurance Fund", Amount = 177 },
                    new PricedItem{ Name = "Gap Cover", Amount = 343 },
                    new PricedItem{ Name = "Canteen", Amount = 300 },
                    new PricedItem{ Name = "Discovery Medical Aid", Amount = 2226 },
                    new PricedItem{ Name = "Discovery Vitality", Amount = 329 },
                    new PricedItem{ Name = "Cool Ideas Fibre", Amount = 1400 },
                    new PricedItem{ Name = "Apple Watch Discovery", Amount = 250 },
                    new PricedItem{ Name = "Santam Insurance", Amount = 2200 },
                    new PricedItem{ Name = "Maid", Amount = 1500 },
                    new PricedItem{ Name = "Fuel & Carwash", Amount = 1900 },
                    new PricedItem{ Name = "Groceries", Amount = 4000 },
                    new PricedItem{ Name = "Apple Music + iCloud", Amount = 150 },
                    new PricedItem{ Name = "Home Levies", Amount = 1350 },
                    new PricedItem{ Name = "Rates, Taxes & Electricity", Amount = 4500 },
                    new PricedItem{ Name = "Pet Supplies", Amount = 1500 },
                    new PricedItem{ Name = "Entertainment", Amount = 3000 },
                    new PricedItem{ Name = "YouTube Premium", Amount = 100 },
                    new PricedItem{ Name = "StressFace", Amount = 50 },
                    new PricedItem{ Name = "Cannibis", Amount = 650 },
                    new PricedItem{ Name = "Infuse Pro", Amount = 50 },
                    new PricedItem{ Name = "Manscaping", Amount = 1000 },
                    new PricedItem{ Name = "Iso Whey Protein & Creatine", Amount = 350 }
                }
            };
            var targetDate = new DateTime(2023, 12, 31);

            // Register custom conditional transactions.
            request.Conditions[isBonusMonthCondition] = new TaxablePricedItem
            {
                Name = "Bonus",
                Amount = 0.7,
                Taxable = true,
                OnceOff = true,
                Type = PricedItemType.SalaryRatio
            };
            request.Conditions[isIncreaseMonth] = new TaxablePricedItem
            {
                Name = "Increase",
                Amount = 0.07,
                Taxable = true,
                Type = PricedItemType.SalaryRatio
            };
            request.Conditions[isLeaveEncashmentMonth] = new TaxablePricedItem
            {
                Name = "Leave Encashment",
                Amount = 1.0 / 21 * 5, // Assuming 21 work days per month and encashing 5 days.
                Taxable = true,
                OnceOff = true,
                Type = PricedItemType.SalaryRatio
            };

            var hasUnsettledAccountsTerminalCondition = new Func<int, DateTime, bool>((monthIndex, monthDate) =>
            {
                var areAllAccountsSettled = request
                                                .Accounts
                                                .All(a => a.RunningBalance >= 0);

                return !areAllAccountsSettled;
            });
            var hasNotMetFinancialGoalsYetTerminalCondition = new Func<int, DateTime, bool>((monthIndex, monthDate) =>
            {
                var hasFinancialGoalsBeenMet = request
                                                .Accounts
                                                .Sum(a => a.RunningBalance) >= 13150000;

                return !hasFinancialGoalsBeenMet;
            });
            var belowZeroNetWorthTerminalCondition = new Func<int, DateTime, bool>((monthIndex, monthDate) =>
            {
                var isNetWorthAboveZero = request
                                                .Accounts
                                                .SelectMany(a => a.Transactions)
                                                .Sum(t => t.Amount) >= 0;

                return !isNetWorthAboveZero;
            });

            // Perform
            //var projectionTillFinancialGoalsMet = await instance.ProjectTillIsTerminalAsync(request, hasNotMetFinancialGoalsYetTerminalCondition);

            // 11 - 2026
            //var projectionForAllDebtSettled = await instance.ProjectTillIsTerminalAsync(request, hasUnsettledAccountsTerminalCondition);

            var projectionForSpecificDate = await instance.ProjectToDateAsync(request, targetDate);

            // 06 - 2025
            //var projectionForWhenNetWorthExceedsZero = await instance.ProjectTillIsTerminalAsync(request, belowZeroNetWorthTerminalCondition);

            // Assert
            Assert.NotNull(projectionForSpecificDate);

            // TODO: Object extension for cloning.
        }
    }
}

