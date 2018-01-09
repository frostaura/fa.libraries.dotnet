using System.Net.Http;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Http.Models.Requests;

namespace FrostAura.Libraries.Http.Extensions
{
    /// <summary>
    /// HttpRequestMessage extensions for making it easier to work with FrostAura HTTPRequest
    /// </summary>
    public static class HttpRequestMessageExtensions
    {
        /// <summary>
        /// Simple converter of HttpRequestMessage to HttpRequest.
        /// </summary>
        /// <param name="requestMessage">The original HttpRequestMessage.</param>
        /// <returns>Instance of converted .HttpRequest</returns>
        public static HttpRequest ToHttpRequest(this HttpRequestMessage requestMessage)
        {
            var httpRequest = new HttpRequest(requestMessage.ThrowIfNull(nameof(requestMessage)));

            return httpRequest;
        }
    }
}