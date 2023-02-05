using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FrostAura.Libraries.MediaServer.Core.Enums;
using FrostAura.Libraries.MediaServer.Core.Models.Content;

namespace FrostAura.Libraries.MediaServer.Core.Interfaces
{
    /// <summary>
    /// All media servers related functions.
    /// </summary>
    public interface IMediaServer<TConfiguration, TServer>
    {
        /// <summary>
        /// Media server specific configuration. Should be set in a constructor.
        /// </summary>
        TConfiguration Configuration { get; }
        
        /// <summary>
        /// Media server initialized to be called before consuming the service.
        /// The setting up of the configuration object should happen in this function.
        /// <param name="serverSelector">Delegate for selecting a server as the default upon discovery.</param>
        /// <param name="token">Cancellation token instance.</param>
        /// </summary>
        Task<InitializationStatus> InitializeAsync(Func<IEnumerable<TServer>, string> serverSelector, CancellationToken token);

        /// <summary>
        /// Collection of libraries from the server
        /// <param name="token">Cancellation token instance.</param>
        /// </summary>
        Task<IEnumerable<Library>> GetAllLibrariesAsync(CancellationToken token);
    }
}