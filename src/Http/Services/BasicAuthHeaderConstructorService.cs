using System;
using System.Collections.Generic;
using System.Text;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Http.Interfaces;
using FrostAura.Libraries.Http.Models.Requests;

namespace FrostAura.Libraries.Http.Services
{
    /// <summary>
    /// Constructor for HTTP headers for basic authorization requests.
    /// </summary>
    public class BasicAuthHeaderConstructorService : IHeaderConstructor<BasicAuthRequest>
    {
        /// <summary>
        /// Construct request headers for HTTP request.
        /// </summary>
        /// <param name="data">Data to be passed to construct headers.</param>
        /// <returns>Headers to be added to HTTP request.</returns>
        public IDictionary<string, string> ConstructRequestHeaders(BasicAuthRequest data)
        {
            data.ThrowIfNull(nameof(data));
            
            var headers = new Dictionary<string, string>();
            var authBytes = Encoding
                .UTF8
                .GetBytes(data.Username.ThrowIfNullOrWhitespace(nameof(data.Username)) + ":" + data.Password.ThrowIfNullOrWhitespace(nameof(data.Password)));
            
            headers.Add("Authorization", "Basic " + Convert.ToBase64String(authBytes));
            
            return headers;
        }
    }
}