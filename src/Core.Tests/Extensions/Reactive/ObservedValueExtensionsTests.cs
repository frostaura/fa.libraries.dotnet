using FrostAura.Libraries.Core.Extensions.Reactive;
using FrostAura.Libraries.Core.Interfaces.Reactive;
using Xunit;

namespace Core.Tests.Extensions.Reactive
{
    public class ObservedValueExtensionsTests
    {
        private class TestObject
        {   
            public string Test { get; set; }
        }
        
        [Fact]
        public void ToObservedValue_WithValidString_ShouldReturnCorrectObservedValue()
        {
            // Setup
            var expected = "test";

            // Perform
            IObservedValue<string> actual = expected
                .AsObservedValue();

            // Assert
            Assert.Equal(expected, actual.Value);
        }
        
        [Fact]
        public void ToObservableValue_WithValidBool_ShouldReturnCorrectObservedValue()
        {
            // Setup
            var expected = true;
            
            // Perform
            IObservedValue<bool> actual = expected
                .AsObservedValue();
            
            // Assert
            Assert.Equal(expected, actual.Value);
        }
        
        [Fact]
        public void ToObservabeValue_WithValidNumber_ShouldReturnCorrectObservedValue()
        {
            // Setup
            var expected = 10;
            
            // Perform
            IObservedValue<int> actual = expected
                .AsObservedValue();
            
            // Assert
            Assert.Equal(expected, actual.Value);
        }
        
        [Fact]
        public void ToObservableValue_WithValidObject_ShouldReturnCorrectObservedValue()
        {
            // Setup
            var expected = new TestObject { Test = "test"};
            
            // Perform
            IObservedValue<TestObject> actual = expected
                .AsObservedValue();
            
            // Assert
            Assert.Equal(expected.Test, actual.Value.Test);
        }
    }
}