using System;
using System.Net.Http;
using FrostAura.Libraries.Http.Models.Requests;
using Xunit;

namespace FrostAura.Libraries.Http.Tests.Extensions
{
    public class HttpRequestMessageExtensionsTests
    {
        [Fact]
        public void ToHttpRequest_WithInvalidHttpRequestMessage_ShouldThrowArgumentNullException()
        {
            // Setup
            
            // Perform action 'ToHttpRequest'

            // Assert that 'ShouldThrowArgumentNullException' = 'WithInvalidHttpRequestMessage'
            Assert.Throws<ArgumentNullException>(() =>
            {
                HttpRequestMessage requestMessage = null;
                HttpRequest httpRequest = Http.Extensions.HttpRequestMessageExtensions.ToHttpRequest(requestMessage);
            });
        }
        
        [Fact]
        public void ToHttpRequest_WithValidRequestMessage_ShouldReturnConvertedHttpRequest()
        {
            // Setup
            var requestMessage = new HttpRequestMessage();
            
            // Perform action 'ToHttpRequest'
            HttpRequest httpRequest = Http.Extensions.HttpRequestMessageExtensions.ToHttpRequest(requestMessage);
            
            // Assert that 'ShouldReturnConvertedHttpRequest' = 'WithValidRequestMessage'
            Assert.NotNull(httpRequest);
            Assert.Equal(requestMessage, httpRequest.Request);
        }
    }
}