using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FrostAura.Libraries.Core.Settings;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Http.Interfaces;
using FrostAura.Libraries.Http.Models.Requests;
using FrostAura.Libraries.Http.Models.Responses;
using Polly;
using Polly.Timeout;

namespace FrostAura.Libraries.Http.Services
{
    /// <summary>
    /// Service responsible for establishing HTTP connections and handling responses.
    /// </summary>
    public class JsonHttpService : IHttpService, IDisposable
    {
        /// <summary>
        /// HTTP client for handling low-level requests.
        /// </summary>
        private HttpClient _httpClient { get; }

        #region Constructors
        
        public JsonHttpService()
        {
            _httpClient = new HttpClient();
        }

        public JsonHttpService(HttpClient client)
        {
            _httpClient = client
                .ThrowIfNull(nameof(client));
        }
        
        #endregion

        /// <summary>
        /// Perform HTTP request and return a JSON response
        /// </summary>
        /// <param name="request">Original HTTP request instance.</param>
        /// <param name="token">Instance of a cancellation token.</param>
        /// <typeparam name="T">Type of the JSON response expected.</typeparam>
        /// <returns>Instance of wrapped HTTP response.</returns>
        public async Task<HttpResponse<T>> RequestAsync<T>(HttpRequest request, CancellationToken token)
        {
            var response = new HttpResponse<T>();
            
            await Policy
                .Handle<Exception>()
                .WaitAndRetry(Resillience.RETRY_COUNT,
                    (i, context) => TimeSpan.FromSeconds(Resillience.RETRY_TIMEOUT))
                .Execute(async () =>
                {
                    HttpResponseMessage responseMessage = await _httpClient
                        .SendAsync(request.Request.ThrowIfNull(nameof(request.Request)), token);

                    await response.SetResponseAsync(request.Identifier, responseMessage, token); 
                });

            return response;
        }
        
        /// <summary>
        /// Displose of all unmanaged resources
        /// </summary>
        public void Dispose()
        {
            _httpClient?
                .Dispose();
        }
    }
}