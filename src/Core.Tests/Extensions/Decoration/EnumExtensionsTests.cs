using System.ComponentModel;
using FrostAura.Libraries.Core.Extensions.Decoration;
using Xunit;

namespace Core.Tests.Extensions.Decoration
{
    internal enum AttributeTest
    {
        NoDescription,
        
        [Description("Test Description")]
        Description,
        
        [Description("{0} + {1} = {2}")]
        WithParameters
    }
    
    public class EnumExtensionsTests
    {
        [Fact]
        public void Description_WithNoCustomAttributeInstance_ShouldReturnNull()
        {
            // Setup
            var enumWithNoAttribute = AttributeTest.NoDescription;

            // Perform action 'Description'
            var enumDescription = enumWithNoAttribute
                .Description();

            // Assert that 'ShouldReturnNull' = 'WithNoCustomAttributeInstance'
            Assert.True(string.IsNullOrWhiteSpace(enumDescription));
        }
        
        [Fact]
        public void Description_WithACustomAttributeInstance_ShouldReturnInstance()
        {
            // Setup
            var enumWithNoAttribute = AttributeTest.Description;
            
            // Perform action 'Description'
            var enumDescription = enumWithNoAttribute
                .Description();
            
            // Assert that 'ShouldReturnInstance' = 'WithACustomAttributeInstance'
            Assert.Equal("Test Description", enumDescription);
        }
        
        [Fact]
        public void Description_WithCustomAttributeInstanceWithPlaceholders_ShouldSubstitutePlaceholders()
        {
            // Setup
            var enumWithNoAttribute = AttributeTest.WithParameters;
            var expectedString = "1 + 2 = 3";
            
            // Perform action 'Description'
            var enumDescription = enumWithNoAttribute
                .Description("1", "2", "3");
            
            // Assert that 'ShouldSubstitutePlaceholders' = 'WithCustomAttributeInstanceWithPlaceholders'
            Assert.Equal(expectedString, enumDescription);
        }
    }
}