using System;
using System.Threading.Tasks;
using FrostAura.Libraries.Core.Abstractions;
using Xunit;

namespace Core.Tests.Abstractions
{
    internal class ObjectWithIdentifier
    {
        public Guid Identifier { get; } = new Guid();
    }
    
    public class SingletonTests
    {
        [Fact]
        public void Singleton_GetInstance_ShouldBeDefined()
        {
            // Setup
            var singletonInstance = new Singleton<ObjectWithIdentifier>();

            // Perform action 'Singleton'
            ObjectWithIdentifier instance = singletonInstance
                .Instance;

            // Assert that 'ShouldBeDefined' = 'GetInstance'
            Assert.NotNull(instance);
        }
        
        [Fact]
        public void Singleton_GetInstanceMultipleTimes_ShouldReturnSameInstance()
        {
            // Setup
            var singletonInstance = new Singleton<ObjectWithIdentifier>();
            
            // Perform action 'Singleton'
            ObjectWithIdentifier instance = singletonInstance
                .Instance;
            ObjectWithIdentifier instance2 = singletonInstance
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
            var singletonInstance = new Singleton<ObjectWithIdentifier>();
            ObjectWithIdentifier instance = null;
            ObjectWithIdentifier instance2 = null;
            
            // Perform action 'Singleton'
            var task = Task.Run(() => instance = singletonInstance.Instance);
            var task2 = Task.Run(() => instance2 = singletonInstance.Instance);

            await Task.WhenAll(task, task2);
            
            // Assert that 'ShouldReturnSameInstance' = 'GetInstanceMultipleTimesAcrossThreads'
            Assert.NotNull(instance);
            Assert.NotNull(instance2);
            
            Assert.Equal(instance.Identifier, instance2.Identifier);
        }
    }
}