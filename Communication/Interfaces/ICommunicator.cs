using System.Threading;
using System.Threading.Tasks;

namespace FrostAura.Libraries.Communication.Interfaces
{
    /// <summary>
    /// Contract for a communication service.
    /// </summary>
    public interface ICommunicator<TRequest, TResponse>
    {
        /// <summary>
        /// Send a message async.
        /// </summary>
        /// <param name="message">Requested message to send.</param>
        /// <param name="token">Cancellation token.</param>
        /// <exception cref="FrostAura.Libraries.Communication.Exceptions.BaseCommunicatorException">When something goes wrong.</exception>
        Task SendMessageAsync(TRequest message, CancellationToken token);

        /// <summary>
        /// Send a message async and wait for a response.
        /// </summary>
        /// <param name="message">Requested message to send.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The response message, if any.</returns>
        /// <exception cref="FrostAura.Libraries.Communication.Exceptions.BaseCommunicatorException">When something goes wrong.</exception>
        Task<TResponse> SendAndWaitForResponseAsync(TRequest message, CancellationToken token);
    }
}
