using Finance.Enums;
using Finance.Models;
using Xunit;

namespace Finance.Tests.Models
{
	public class AccountTests
	{
        [Fact]
        public void Constructor_WithValidName_ShouldThrow()
        {
            string name = default;

            var actual = Assert.Throws<ArgumentNullException>(() => new Account(name));

            Assert.Equal(nameof(name), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidInitialBalance_ShouldConstructAndAddInitialTransaction()
        {
            var expected = 1234.567;
            var name = "Test Account";
            var actual = new Account(name, balance: expected);

            Assert.Single(actual.Transactions);
            Assert.Equal(expected, actual.Transactions.First().Amount);
        }

        [Fact]
		public void Constructor_WithValidType_ShouldConstructAndSetType()
		{
            var expected = AccountType.Repeat;
            var name = "Test Account";
			var actual = new Account(name, accountType: expected);

			Assert.NotNull(actual);
            Assert.Equal(expected, actual.Type);
		}

        [Fact]
        public void Constructor_WithValidArgs_ShouldHaveNoTransactions()
        {
            var name = "Test Account";
            var instance = new Account(name);

			Assert.NotNull(instance.Transactions);
            Assert.Empty(instance.Transactions);
			Assert.Equal(0, instance.Balance);
        }
    }
}
