using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FrostAura.Libraries.MediaServer.Core.Models.Content;

namespace MediaServer.Plex.Interfaces
{
    /// <summary>
    /// Provider for plex libraries.
    /// </summary>
    public interface IPlexMediaProvider
    {
        /// <summary>
        /// Collection of libraries from the server
        /// <param name="token">Cancellation token instance.</param>
        /// </summary>
        Task<IEnumerable<Library>> GetAllLibrariesAsync(CancellationToken token);
    }
}