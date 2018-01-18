using FrostAura.Libraries.Core.Extensions.Reactive;
using Xunit;

namespace Core.Tests.Models.Reactive
{
    public class ObservedTests
    {
        [Fact]
        public void Subscribe_WithNoValue_ShouldNotCallHandler()
        {
            // Setup
            var instance = "test"
                .AsObservedValue();
            var isHandlerCalled = false; 
            
            instance.Value = null;

            // Perform
            instance.Subscribe((value) =>
            {
                isHandlerCalled = true;
            });

            // Assert
            Assert.False(isHandlerCalled);
        }
        
        [Fact]
        public void Subscribe_WithValidValue_ShouldCallHandler()
        {
            // Setup
            var instance = "test"
                .AsObservedValue();
            var isHandlerCalled = false; 
            
            instance.Value = "test2";

            // Perform
            instance.Subscribe((value) =>
            {
                isHandlerCalled = true;
                
                Assert.Equal(instance.Value, value);
            });

            // Assert
            Assert.True(isHandlerCalled);
        }
    }
}