using System;
using FrostAura.Libraries.Core.Extensions.Validation;
using Xunit;

namespace Core.Tests.Extensions.Validation
{
    public class StringExtensionsTests
    {
        [Fact]
        public void ThrowIfNull_WhileDefined_ShouldNotThrow()
        {
            // Setup
            var str = "test string";

            // Perform action 'ThrowIfNull'
            var returnedStr = str.ThrowIfNullOrWhitespace(nameof(str));

            // Assert that 'ShouldNotThrow' = 'WhileDefined'
            Assert.Equal(str, returnedStr);
        }
        
        [Fact]
        public void ThrowIfNull_WhileNotDefined_ShouldThrowArgumentNullException()
        {
            // Setup
            string str = null;

            // Perform action 'ThrowIfNull'
            var exception = Assert
                .Throws<ArgumentNullException>(() => str.ThrowIfNullOrWhitespace(nameof(str)));

            // Assert that 'ShouldThrowArgumentNullException' = 'WhileNotDefined'
            Assert.NotNull(exception);
            Assert.Equal(nameof(str), exception.ParamName);
        }
    }
}