using System.Collections.Generic;
using System.Linq;
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
        
        /// <summary>
        /// Add an accept json header to the request message headers.
        /// </summary>
        /// <param name="request">Original request message.</param>
        /// <returns>The ammended request message.</returns>
        public static HttpRequestMessage AcceptJson(this HttpRequestMessage request)
        {
            request.Headers.Add("Accept", "application/json");

            return request;
        }

        /// <summary>
        /// Add or update request headers.
        /// </summary>
        /// <param name="request">Original request message.</param>
        /// <param name="headers">Headers to add or update on the request object.</param>
        /// <returns>The ammended request message.</returns>
        public static HttpRequestMessage AddRequestHeaders(this HttpRequestMessage request, IDictionary<string, string> headers)
        {
            foreach (KeyValuePair<string, string> currentHeader in headers.ThrowIfNull(nameof(headers)))
            {
                if (request.Headers.Contains(currentHeader.Key))
                {
                    request.Headers.Remove(currentHeader.Key);
                }
                
                request.Headers.Add(currentHeader.Key, currentHeader.Value);
            }

            return request;
        }
    }
}