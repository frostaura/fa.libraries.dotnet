using System;
using System.Diagnostics;
using System.Net.Http;
using FrostAura.Libraries.Core.Extensions.Validation;

namespace FrostAura.Libraries.Http.Models.Requests
{
    /// <summary>
    /// Wrapping HTTP request
    /// </summary>
    [DebuggerDisplay("Request Id: {Identifier}")]
    public class HttpRequest
    {
        /// <summary>
        /// Unique request identifier.
        /// </summary>
        public Guid Identifier { get; } = Guid.NewGuid();
        
        /// <summary>
        /// Original request message instance.
        /// </summary>
        private HttpRequestMessage _request { get; }

        /// <summary>
        /// Overloaded consutructor to access original request message.
        /// </summary>
        /// <param name="request"></param>
        public HttpRequest(HttpRequestMessage request)
        {
            _request = request
                .ThrowIfNull(nameof(request));
        }

        /// <summary>
        /// Property to grab a transformed request.
        /// </summary>
        public HttpRequestMessage Request => _request;

        /// <summary>
        /// Override equals to support unit testing and linq.
        /// </summary>
        /// <param name="obj">Object to compare to.</param>
        /// <returns>Whether or not the two objects are equal.</returns>
        public override bool Equals(object obj)
        {
            var castedRequest = obj.ThrowIfNull<object>(nameof(obj)) as HttpRequest;
            bool areEqual = Identifier.Equals(castedRequest?.Identifier);

            return areEqual;
        }

        /// <summary>
        /// Override the getting of hash code for what makes the request unique.
        /// </summary>
        /// <returns>The unique hashcode.</returns>
        public override int GetHashCode()
        {
            return Identifier.GetHashCode();
        }
    }
}