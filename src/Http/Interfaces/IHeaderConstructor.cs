using System.Collections.Generic;

namespace FrostAura.Libraries.Http.Interfaces
{
    /// <summary>
    /// For services that are responsible for constructing headers for a request.
    /// </summary>
    public interface IHeaderConstructor<T>
    {
        /// <summary>
        /// Construct request headers for HTTP request.
        /// </summary>
        /// <param name="data">Data to be passed to construct headers.</param>
        /// <returns>Headers to be added to HTTP request.</returns>
        IDictionary<string, string> ConstructRequestHeaders(T data);
    }
}