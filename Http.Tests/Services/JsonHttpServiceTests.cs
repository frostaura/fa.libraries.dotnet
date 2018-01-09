using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FrostAura.Libraries.Http.Interfaces;
using FrostAura.Libraries.Http.Models.Requests;
using FrostAura.Libraries.Http.Models.Responses;
using FrostAura.Libraries.Http.Services;
using FrostAura.Libraries.Http.Tests.Models.Responses;
using Newtonsoft.Json;
using NSubstitute;
using NSubstitute.Extensions;
using Xunit;

namespace FrostAura.Libraries.Http.Tests.Services
{
    public class JsonHttpServiceTests
    {
        internal class TestClass
        {
            public string Name { get; set; }
        }
        
        [Fact]
        public void Constructor_WithNoParams_ShouldConstruct()
        {
            // Setup
            
            // Perform action 'Constructor'
            IHttpService actual = new JsonHttpService();
            
            // Assert that 'ShouldConstruct' = 'WithNoParams'
            Assert.NotNull(actual);
        }
        
        [Fact]
        public void Constructor_WithHttpClientParam_ShouldConstruct()
        {
            // Setup
            var httpClient = Substitute.For<HttpClient>();
            
            // Perform action 'Constructor'
            IHttpService actual = new JsonHttpService(httpClient);
            
            // Assert that 'ShouldConstruct' = 'WithHttpClientParam'
            Assert.NotNull(actual);
        }
        
        [Fact]
        public void Dispose_WithNoParameters_ShouldDisposeHttpClient()
        {
            // Setup
            var httpClient = Substitute.For<HttpClient, IDisposable>();
            IHttpService service = new JsonHttpService(httpClient);
            
            // Perform action 'Dispose'
            (service as IDisposable).Dispose();
            
            // Assert that 'ShouldDisposeHttpClient' = 'WithNoParameters'
            httpClient
                .Received()
                .Dispose();
        }
        
        [Fact]
        public void Request_WithInvalidRequest_ShouldThrowArgumentNullException()
        {
            // Setup
            var httpClient = Substitute.For<HttpClient, IDisposable>();
            IHttpService service = new JsonHttpService(httpClient);
            HttpRequest request = null;

            // Perform action 'Request'
            var exception = Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await service.RequestAsync<TestObject>(request, default(CancellationToken));
            });

            // Assert that 'ShouldThrowArgumentNullException' = 'WithInvalidRequest'
        }
        
        [Fact]
        public async Task Request_WithValidRequest_ShouldCallSendAsyncOnHttpClient()
        {
            // Setup
            var httpClient = Substitute.For<HttpClient, IDisposable>();
            IHttpService service = new JsonHttpService(httpClient);
            
            var httpRequestMessage = new HttpRequestMessage();
            HttpRequest request = new HttpRequest(httpRequestMessage);
            
            var expectedResponse = new TestClass { Name = "Test class"};
            var originalResponse = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedResponse))
            };
            var response = new HttpResponse<TestClass>();

            await response.SetResponseAsync(Guid.NewGuid(), originalResponse, default(CancellationToken));
            
            httpClient
                .SendAsync(httpRequestMessage, default(CancellationToken))
                .Returns(originalResponse);
            
            // Perform action 'Request'
            await service.RequestAsync<TestObject>(request, default(CancellationToken));
            
            // Assert that 'ShouldCallSendAsyncOnHttpClient' = 'WithValidRequest'
            httpClient
                .Received()
                .SendAsync(httpRequestMessage, default(CancellationToken));
        }
        
        [Fact]
        public async Task Request_WithValidRequest_ShouldReturnCastedResponse()
        {
            // Setup
            var httpClient = Substitute.For<HttpClient, IDisposable>();
            IHttpService service = new JsonHttpService(httpClient);
            
            var httpRequestMessage = new HttpRequestMessage();
            HttpRequest request = new HttpRequest(httpRequestMessage);
            
            var expectedResponse = new TestClass { Name = "Test class"};
            var originalResponse = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedResponse))
            };
            var response = new HttpResponse<TestClass>();

            await response.SetResponseAsync(request.Identifier, originalResponse, default(CancellationToken));
            
            httpClient
                .SendAsync(httpRequestMessage, default(CancellationToken))
                .Returns(originalResponse);
            
            // Perform action 'Request'
            var actual = await service.RequestAsync<TestObject>(request, default(CancellationToken));
            
            // Assert that 'ShouldCallSendAsyncOnHttpClient' = 'WithValidRequest'
            Assert.Equal(expectedResponse.Name, actual.Response.Name);
            Assert.Equal(request.Identifier, response.RequestIdentifier);
        }
    }
}