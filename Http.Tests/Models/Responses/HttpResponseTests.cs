using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FrostAura.Libraries.Http.Models.Responses;
using Newtonsoft.Json;
using Xunit;

namespace FrostAura.Libraries.Http.Tests.Models.Responses
{
    internal class TestObject
    {
        public string Name { get; set; }
    }
    
    public class HttpResponseTests
    {
        [Fact]
        public void Constructor_Default_ShouldConstruct()
        {
            // Setup
            
            // Perform action 'Constructor'
            var actual = new HttpResponse<string>();

            // Assert that 'ShouldConstruct' = 'Default'
            Assert.NotNull(actual);
            Assert.Null(actual.Response);
            Assert.Null(actual.ResponseMessage);
        }
        
        [Fact]
        public async Task SetResponse_WithInvalidResponseMessage_ShouldThrowArgumentNullException()
        {
            // Setup
            var instance = new HttpResponse<string>();
            
            // Perform action 'SetResponse'
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                await instance.SetResponseAsync(Guid.NewGuid(), null, default(CancellationToken));
            });

            // Assert that 'ShouldThrowArgumentNullException' = 'WithInvalidResponseMessage'
        }
        
        [Fact]
        public async Task SetResponse_WithNon200StatusCode_ShouldNotSetCastedResponse()
        {
            // Setup
            var instance = new HttpResponse<string>();
            var requestMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
            
            // Perform action 'SetResponse'
            await instance.SetResponseAsync(Guid.NewGuid(), requestMessage, default(CancellationToken));
            
            // Assert that 'ShouldNotSetCastedResponse' = 'WithNon200StatusCode'
            Assert.Null(instance.Response);
            Assert.NotNull(instance.ResponseMessage);
        }
        
        [Fact]
        public async Task SetResponse_With200StatusCode_ShouldSetCastedResponse()
        {
            // Setup
            var instance = new HttpResponse<TestObject>();
            var expected = new TestObject { Name = "Test response" };
            var requestMessage = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expected))
            };
            
            // Perform action 'SetResponse'
            await instance.SetResponseAsync(Guid.NewGuid(), requestMessage, default(CancellationToken));
            
            // Assert that 'ShouldSetCastedResponse' = 'With200StatusCode'
            Assert.NotNull(instance.Response);
            Assert.NotNull(instance.ResponseMessage);
            Assert.Equal(expected.Name, instance.Response.Name);
        }
    }
}