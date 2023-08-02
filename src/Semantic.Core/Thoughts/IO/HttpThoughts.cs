using FrostAura.Libraries.Core.Extensions.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.SkillDefinition;
using System.ComponentModel;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.IO
{
	/// <summary>
	/// Http thoughts.
	/// </summary>
	public class HttpThoughts : BaseThought
    {
        /// <summary>
        /// HTTP client factory.
        /// </summary>
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="httpClientFactory">HTTP client factory.</param>
        /// <param name="logger">Instance logger.</param>
        public HttpThoughts(IHttpClientFactory httpClientFactory, ILogger<HttpThoughts> logger)
            :base(logger)
        {
            _httpClientFactory = httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
        }

        /// <summary>
        /// Sends an HTTP GET request to the specified URI and returns the response body as a string.
        /// </summary>
        /// <param name="uri">URI of the request.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>The response body as a string.</returns>
        [SKFunction, Description("Makes a HTTP GET request to a URI.")]
        public Task<string> GetAsync(
            [Description("The URI of the request.")] string uri,
            CancellationToken token = default) =>
            SendRequestAsync(uri, HttpMethod.Get, requestContent: null, token);

        /// <summary>
        /// Sends an HTTP POST request to the specified URI and returns the response body as a string.
        /// </summary>
        /// <param name="uri">URI of the request.</param>
        /// <param name="body">The body of the request.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>The response body as a string.</returns>
        [SKFunction, Description("Makes a HTTP POST request to a URI.")]
        public Task<string> PostAsync(
            [Description("The URI of the request.")] string uri,
            [Description("The body of the request.")] string body,
            CancellationToken token = default) =>
            SendRequestAsync(uri, HttpMethod.Post, new StringContent(body), token);

        /// <summary>
        /// Sends an HTTP PUT request to the specified URI and returns the response body as a string.
        /// </summary>
        /// <param name="uri">URI of the request.</param>
        /// <param name="body">The body of the request.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>The response body as a string.</returns>
        [SKFunction, Description("Makes a HTTP PUT request to a URI.")]
        public Task<string> PutAsync(
            [Description("The URI of the request.")] string uri,
            [Description("The body of the request.")] string body,
            CancellationToken token = default) =>
            SendRequestAsync(uri, HttpMethod.Put, new StringContent(body), token);

        /// <summary>
        /// Sends an HTTP DELETE request to the specified URI and returns the response body as a string.
        /// </summary>
        /// <param name="uri">URI of the request.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>The response body as a string.</returns>
        [SKFunction, Description("Makes a HTTP DELETE request to a URI.")]
        public Task<string> DeleteAsync(
            [Description("The URI of the request.")] string uri,
            CancellationToken token = default) =>
            SendRequestAsync(uri, HttpMethod.Delete, requestContent: null, token);

        /// <summary>Sends an HTTP request and returns the response content as a string.</summary>
        /// <param name="uri">The URI of the request.</param>
        /// <param name="method">The HTTP method for the request.</param>
        /// <param name="requestContent">Optional request content.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        private async Task<string> SendRequestAsync(string uri, HttpMethod method, HttpContent? requestContent, CancellationToken token)
        {
            using var client = _httpClientFactory.CreateClient();
            using var request = new HttpRequestMessage(method, uri.ThrowIfNullOrWhitespace(nameof(uri))) { Content = requestContent };
            using var response = await client.SendAsync(request, token);

            return await response.Content.ReadAsStringAsync();
        }
    }
}
