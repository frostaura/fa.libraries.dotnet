using System.Threading;
using System.Threading.Tasks;
using MediaServer.Plex.Models.Config;

namespace MediaServer.Plex.Interfaces
{
    /// <summary>
    /// Plex server settings and preferences provider service.
    /// </summary>
    public interface IPlexServerSettingsProvider
    {
        /// <summary>
        /// Fetch server settings and preferences async.
        /// </summary>
        /// <param name="token">Cancellation token.</param>
        /// <returns>Server preferences and settings.</returns>
        Task<ServerPreferences> GetServerSettingsAsync(CancellationToken token);
    }
}