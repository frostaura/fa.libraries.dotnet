using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FrostAura.Libraries.Core.Extensions.Validation;
using Newtonsoft.Json;

namespace FrostAura.Libraries.Http.Models.Responses
{
    /// <summary>
    /// Wrapping HTTP response.
    /// </summary>
    /// <typeparam name="T">Type of the JSON response expected.</typeparam>
    [DebuggerDisplay("Request Id: {RequestIdentifier}")]
    public class HttpResponse<T>
    {
        /// <summary>
        /// Unique request identifier.
        /// </summary>
        public Guid RequestIdentifier { get; private set; }
        
        /// <summary>
        /// Casted response to specified type.
        /// </summary>
        public T Response { get; private set; }
        
        /// <summary>
        /// Original HTTP response instance.
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; private set; }

        /// <summary>
        /// Read, cast and set the response.
        /// </summary>
        /// <param name="requestIdentifier">Identifier of the request.</param>
        /// <param name="responseMessage">Original HTTP response instance.</param>
        /// <param name="token">Instance of a cancellation token.</param>
        public async Task SetResponseAsync(Guid requestIdentifier, HttpResponseMessage responseMessage, CancellationToken token)
        {
            ResponseMessage = responseMessage
                .ThrowIfNull(nameof(responseMessage));

            RequestIdentifier = requestIdentifier;
            
            if ((int)responseMessage.StatusCode >= 200 && (int)responseMessage.StatusCode < 300)
            {
               await ProcessResponseCasting(token); 
            }
            else
            {
                Console.WriteLine($"HTTP Response {(int)responseMessage.StatusCode}");
            }
        }

        /// <summary>
        /// Cast the string response to the type of T.
        /// <param name="token">Instance of a cancellation token.</param>
        /// </summary>
        private async Task ProcessResponseCasting(CancellationToken token)
        {
            string stringResponse = await ResponseMessage
                .Content
                .ReadAsStringAsync();

            if (typeof(T) == typeof(string)) return;
            
            Response = JsonConvert
                .DeserializeObject<T>(stringResponse);

            var validationErrors = new List<ValidationResult>();
            
            if (!Response.IsValid(out validationErrors))
            {
                Console.WriteLine($"HTTP Response was {ResponseMessage.StatusCode} but the deserialized model contained {validationErrors.Count} validation errors.");
            }
        }
    }
}