using System;
using System.Collections.Generic;
using FrostAura.Libraries.Http.Interfaces;
using FrostAura.Libraries.Http.Models.Requests;
using FrostAura.Libraries.Http.Services;
using Xunit;

namespace FrostAura.Libraries.Http.Tests.Services
{
    public class BasicAuthHeaderConstructorServiceTests
    {
        [Fact]
        public void ConstructRequestHeaders_WithInvalidData_ShouldThrowArgumentNullException()
        {
            // Setup
            BasicAuthRequest request = null;
            IHeaderConstructor<BasicAuthRequest> instance = new BasicAuthHeaderConstructorService();

            // Perform
            var exception = Assert.Throws<ArgumentNullException>(() => instance.ConstructRequestHeaders(request));

            // Assert
            Assert.Equal("data", exception.ParamName);
        }
        
        [Fact]
        public void ConstructRequestHeaders_WithInvalidUsername_ShouldThrowArgumentNullException()
        {
            // Setup
            var request = new BasicAuthRequest
            {
                Password = "test"
            };
            IHeaderConstructor<BasicAuthRequest> instance = new BasicAuthHeaderConstructorService();
            
            // Perform
            var exception = Assert.Throws<ArgumentNullException>(() => instance.ConstructRequestHeaders(request));
            
            // Assert
            Assert.Equal("Username", exception.ParamName);
        }
        
        [Fact]
        public void ConstructRequestHeaders_WithInvalidPassword_ShouldThrowArgumentNullException()
        {
            // Setup
            var request = new BasicAuthRequest
            {
                Username = "test"
            };
            IHeaderConstructor<BasicAuthRequest> instance = new BasicAuthHeaderConstructorService();
            
            // Perform
            var exception = Assert.Throws<ArgumentNullException>(() => instance.ConstructRequestHeaders(request));
            
            // Assert
            Assert.Equal("Password", exception.ParamName);
        }
        
        [Fact]
        public void ConstructRequestHeaders_WithValidRequest_ShouldConstructHeaders()
        {
            // Setup
            var expectedHeader = "Authorization";
            var expectedValue = "Basic dGVzdCB1c2VyOnRlc3QgcGFzc3dvcmQ=";
            var request = new BasicAuthRequest
            {
                Username = "test user",
                Password = "test password"
            };
            IHeaderConstructor<BasicAuthRequest> instance = new BasicAuthHeaderConstructorService();
            
            // Perform
            IDictionary<string, string> actual = instance.ConstructRequestHeaders(request);

            // Assert
            Assert.True(actual.ContainsKey(expectedHeader));
            Assert.Equal(expectedValue, actual[expectedHeader]);
        }
    }
}