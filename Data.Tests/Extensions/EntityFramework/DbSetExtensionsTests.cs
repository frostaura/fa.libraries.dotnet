using System.Collections.Generic;
using System.Linq;
using FrostAura.Libraries.Data.Extensions.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using NSubstitute;
using Xunit;

namespace FrostAura.Libraries.Data.Tests.Extensions.EntityFramework
{
    public class DbSetExtensionsTests
    {
        [Fact]
        public void GetOrAdd_WithNoExistingEntity_ShouldAddAndReturn()
        {
            // Setup
            var collection = Substitute.For<DbSet<string>, IQueryable<string>>();
            var toAdd = "Test to add";
            var fakeData = new List<string>().AsQueryable();
            
            ((IQueryable<string>)collection).Provider.Returns(fakeData.Provider);
            ((IQueryable<string>)collection).Expression.Returns(fakeData.Expression);
            ((IQueryable<string>)collection).ElementType.Returns(fakeData.ElementType);
            ((IQueryable<string>)collection).GetEnumerator().Returns(fakeData.GetEnumerator());
            
            // Perform action 'GetOrAdd'
            var addedEntity = collection.GetOrAdd(toAdd, s => s.Equals(toAdd));

            // Assert that 'ShouldAddAndReturn' = 'WithNoExistingEntity'
            collection.Received().Add(toAdd);
        }
        
        [Fact]
        public void GetOrAdd_WithAlreadyExistingEntity_ShouldReturnExistingEntity()
        {
            // Setup
            var collection = Substitute.For<DbSet<string>, IQueryable<string>>();
            var toAdd = "Test to add";
            var fakeData = (new List<string> { toAdd }).AsQueryable();
            
            ((IQueryable<string>)collection).Provider.Returns(fakeData.Provider);
            ((IQueryable<string>)collection).Expression.Returns(fakeData.Expression);
            ((IQueryable<string>)collection).ElementType.Returns(fakeData.ElementType);
            ((IQueryable<string>)collection).GetEnumerator().Returns(fakeData.GetEnumerator());
            
            // Perform action 'GetOrAdd'
            var addedEntity = collection.GetOrAdd(toAdd, s => s.Equals(toAdd));
            
            // Assert that 'ShouldReturnExistingEntity' = 'WithAlreadyExistingEntity'
            collection.DidNotReceive().Add(toAdd);
        }
    }
}