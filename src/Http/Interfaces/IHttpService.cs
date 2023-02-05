using System.Threading;
using System.Threading.Tasks;
using FrostAura.Libraries.Http.Models.Requests;
using FrostAura.Libraries.Http.Models.Responses;

namespace FrostAura.Libraries.Http.Interfaces
{
    /// <summary>
    /// Services responsible for establishing HTTP connections and handling responses.
    /// </summary>
    public interface IHttpService
    {
        /// <summary>
        /// Perform HTTP request and return a JSON response
        /// </summary>
        /// <param name="request">Original HTTP request instance.</param>
        /// <param name="token">Instance of a cancellation token.</param>
        /// <typeparam name="T">Type of the JSON response expected.</typeparam>
        /// <returns>Instance of wrapped HTTP response.</returns>
        Task<HttpResponse<T>> RequestAsync<T>(HttpRequest request, CancellationToken token);
    }
}