using System;
using System.Net.Http;
using FrostAura.Libraries.Http.Models.Requests;
using NSubstitute;
using Xunit;

namespace FrostAura.Libraries.Http.Tests.Models.Requests
{
    public class HttpRequestTests
    {
        [Fact]
        public void Constructor_WithInvalidRequest_ShouldThrowArgumentNullException()
        {
            // Setup
            
            // Perform action 'Constructor'

            // Assert that 'ShouldThrowArgumentNullException' = 'WithInvalidRequest'
            var exception = Assert.Throws<ArgumentNullException>(() =>
            {
                var actual = new HttpRequest(null);
            });
        }
        
        [Fact]
        public void Constructor_WithValidRequest_ShouldConstruct()
        {
            // Setup
            var request = Substitute.For<HttpRequestMessage>();
            
            // Perform action 'Constructor'
            var actual = new HttpRequest(request);
            
            // Assert that 'ShouldConstruct' = 'WithValidRequest'
            Assert.NotNull(actual);
        }
        
        [Fact]
        public void GetRequest_ShouldReturn_Request()
        {
            // Setup
            var request = Substitute.For<HttpRequestMessage>();
            var actual = new HttpRequest(request);
            
            // Perform action 'GetRequest'
            HttpRequestMessage extractedRequest = actual
                .Request;

            // Assert that 'Request' = 'ShouldReturn'
            Assert.Equal(request, extractedRequest);
        }
    }
}