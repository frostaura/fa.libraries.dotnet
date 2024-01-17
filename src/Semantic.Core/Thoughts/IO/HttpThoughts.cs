using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.IO;

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
    /// <param name="serviceProvider">The dependency service provider.</param>
    /// <param name="semanticKernelLanguageModels">A component for communicating with language models.</param>
    /// <param name="httpClientFactory">HTTP client factory.</param>
    /// <param name="logger">Instance logger.</param>
    public HttpThoughts(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, IHttpClientFactory httpClientFactory, ILogger<HttpThoughts> logger)
        : base(serviceProvider, semanticKernelLanguageModels, logger)
    {
        _httpClientFactory = httpClientFactory.ThrowIfNull(nameof(httpClientFactory));
    }

    /// <summary>
    /// Sends an HTTP GET request to the specified URI and returns the response body as a string.
    /// </summary>
    /// <param name="uri">URI of the request.</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The response body as a string.</returns>
    [KernelFunction, Description("Makes a raw HTTP GET request to a URI and return the string content.")]
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
    [KernelFunction, Description("Makes a raw HTTP POST request to a URI and return the string content.")]
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
    [KernelFunction, Description("Makes a raw HTTP PUT request to a URI and return the string content.")]
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
    [KernelFunction, Description("Makes a raw HTTP DELETE request to a URI and return the string content.")]
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
