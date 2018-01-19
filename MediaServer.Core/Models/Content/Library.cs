using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using FrostAura.Libraries.MediaServer.Core.Enums;

namespace FrostAura.Libraries.MediaServer.Core.Models.Content
{
    /// <summary>
    /// Generic library model.
    /// </summary>
    [DebuggerDisplay("{Title} - {Type}")]
    public class Library
    {
        /// <summary>
        /// Id of the library.
        /// </summary>
        public string Id { get; set; }
        
        /// <summary>
        /// The short description of the library.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The library thumbnail full URL.
        /// </summary>
        public string Thumbnail { get; set; }

        /// <summary>
        /// The library poster full URL.
        /// </summary>
        public string Poster { get; set; }

        /// <summary>
        /// Type of library.
        /// </summary>
        public LibraryType Type { get; set; }

        #region Get Library Specific Types 
        
        /// <summary>
        /// Awaitable movies collection.
        /// </summary>
        public Task<IEnumerable<Movie>> GetMoviesAsync;

        #endregion
    }
}