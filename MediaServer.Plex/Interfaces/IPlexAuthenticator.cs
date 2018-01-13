using System.Threading;
using System.Threading.Tasks;
using MediaServer.Plex.Models.Responses;

namespace MediaServer.Plex.Interfaces
{
    /// <summary>
    /// Authenticator interface for Plex.
    /// </summary>
    public interface IPlexAuthenticator
    {
        /// <summary>
        /// Authenticate a user using username and password.
        /// </summary>
        /// <param name="token">Cancellation token instance.</param>
        /// <returns>Response for authentication whether success or failure.</returns>
        Task<UserAuthenticationResponse> AuthenticateAsync(CancellationToken token);
    }
}