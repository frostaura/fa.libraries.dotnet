using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediaServer.Core.Enums;
using MediaServer.Core.Models.Content;

namespace MediaServer.Core.Interfaces
{
    /// <summary>
    /// All media servers related functions.
    /// </summary>
    public interface IMediaServer<T>
    {
        /// <summary>
        /// Media server specific configuration. Should be set in a constructor.
        /// </summary>
        T Configuration { get; }
        
        /// <summary>
        /// Media server initialized to be called before consuming the service.
        /// The setting up of the configuration object should happen in this function.
        /// <param name="token">Cancellation token instance.</param>
        /// </summary>
        Task<InitializationStatus> InitializeAsync(CancellationToken token);

        /// <summary>
        /// Collection of libraries from the server
        /// <param name="token">Cancellation token instance.</param>
        /// </summary>
        Task<IEnumerable<Library>> GetAllLibrariesAsync(CancellationToken token);
    }
}