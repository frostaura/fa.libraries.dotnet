using System;
using System.Threading.Tasks;
using FrostAura.Libraries.Core.Abstractions;
using Xunit;

namespace Core.Tests.Abstractions
{
    internal class ObjectWithIdentifier : Singleton<ObjectWithIdentifier>
    {
        public Guid Identifier { get; } = new Guid();
    }
    
    public class SingletonTests
    {
        [Fact]
        public void Singleton_GetInstance_ShouldBeDefined()
        {
            // Setup
            // Perform action 'Singleton'
            ObjectWithIdentifier instance = ObjectWithIdentifier
                .Instance;

            // Assert that 'ShouldBeDefined' = 'GetInstance'
            Assert.NotNull(instance);
        }
        
        [Fact]
        public void Singleton_GetInstanceMultipleTimes_ShouldReturnSameInstance()
        {
            // Setup
            // Perform action 'Singleton'
            ObjectWithIdentifier instance = ObjectWithIdentifier
                .Instance;
            ObjectWithIdentifier instance2 = ObjectWithIdentifier
                .Instance;
            
            // Assert that 'ShouldReturnSameInstance' = 'GetInstanceMultipleTimes'
            Assert.NotNull(instance);
            Assert.NotNull(instance2);
            
            Assert.Equal(instance.Identifier, instance2.Identifier);
        }
        
        [Fact]
        public async Task Singleton_GetInstanceMultipleTimesAcrossThreads_ShouldReturnSameInstance()
        {
            // Setup
            ObjectWithIdentifier instance = null;
            ObjectWithIdentifier instance2 = null;
            
            // Perform action 'Singleton'
            var task = Task.Run(() => instance = ObjectWithIdentifier.Instance);
            var task2 = Task.Run(() => instance2 = ObjectWithIdentifier.Instance);

            await Task.WhenAll(task, task2);
            
            // Assert that 'ShouldReturnSameInstance' = 'GetInstanceMultipleTimesAcrossThreads'
            Assert.NotNull(instance);
            Assert.NotNull(instance2);
            
            Assert.Equal(instance.Identifier, instance2.Identifier);
        }
    }
}