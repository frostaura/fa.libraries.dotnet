using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using FrostAura.Libraries.Http.Extensions;
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
        
        [Fact]
        public void AcceptJson_WithNoParams_ShouldAppendAcceptJsonHeader()
        {
            // Setup
            var request = new HttpRequestMessage();
            
            // Perform action 'AcceptJson'
            request.AcceptJson();

            var header = request.Headers.FirstOrDefault(h => h.Key == "Accept");
            
            // Assert that 'ShouldAppendAcceptJsonHeader' = 'WithNoParams'
            Assert.NotNull(header);
            Assert.Equal("application/json", header.Value.FirstOrDefault());
        }
        
        [Fact]
        public void AddRequestHeaders_WithInvalidHeaders_ShouldThrowArgumaentNullException()
        {
            // Setup
            var request = new HttpRequestMessage();
            IDictionary<string, string> headers = null;
            
            // Perform
            
            // Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                HttpRequestMessage ammendedRequest = request.AddRequestHeaders(headers);
            });
        }
        
        [Fact]
        public void AddRequestHeaders_WithValidheaders_ShouldAddHeadersToRequest()
        {
            // Setup
            var request = new HttpRequestMessage();
            var expected = new KeyValuePair<string, string>("test_key", "test_value");
            IDictionary<string, string> headers = new Dictionary<string, string>();
            
            headers.Add(expected);
            
            // Perform
            request.AddRequestHeaders(headers);
            
            // Assert
            Assert.Equal(expected.Value, request.Headers.First(h => h.Key == expected.Key).Value.First());
        }
        
        [Fact]
        public void AddRequestHeaders_WithAlreadyExistingHeader_ShouldUpdateHeaderValue()
        {
            // Setup
            var request = new HttpRequestMessage();
            var existing = new KeyValuePair<string, string>("test_key", "test_value_updated");
            var expected = new KeyValuePair<string, string>("test_key", "test_value_updated");
            IDictionary<string, string> headersExisting = new Dictionary<string, string>();
            IDictionary<string, string> headers = new Dictionary<string, string>();
            
            headersExisting.Add(existing);
            headers.Add(expected);
            
            // Perform
            request.AddRequestHeaders(headersExisting);
            request.AddRequestHeaders(headers);
            
            // Assert
            Assert.Equal(expected.Value, request.Headers.First(h => h.Key == expected.Key).Value.First());
        }
    }
}