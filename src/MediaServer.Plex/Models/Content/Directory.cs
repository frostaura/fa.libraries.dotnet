using System.Collections.Generic;
using System.Diagnostics;

namespace MediaServer.Plex.Models.Content
{
    /// <summary>
    /// Library / Directory model.
    /// </summary>
    [DebuggerDisplay("Title: {Title}")]
    public class Directory
    {
        /// <summary>
        /// Whether or not async is allowed.
        /// </summary>
        public bool AllowAsync { get; set; }
        
        /// <summary>
        /// Artwork for the directory relative to the root of the server.
        /// </summary>
        public string Art { get; set; }

        /// <summary>
        /// Composite
        /// </summary>
        public string Composite { get; set; }

        /// <summary>
        /// Whether or not filters are supported.
        /// </summary>
        public bool Filters { get; set; }

        /// <summary>
        /// Whether or not the library is currently being refreshed.
        /// </summary>
        public bool Refreshing { get; set; }

        /// <summary>
        /// Thumbnail for the directory relative to the root of the server.
        /// </summary>
        public string Thumb { get; set; }

        /// <summary>
        /// The key for the collection. This is used for making calls to get media for the library.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The type of the directory.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The title of the directory | Short description.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The agent assigned to gather media information for this directory.
        /// </summary>
        public string Agent { get; set; }

        /// <summary>
        /// The scanner assigned to gather media information for this directory.
        /// </summary>
        public string Scanner { get; set; }

        /// <summary>
        /// The language this directory is assigned.
        /// </summary>
        public string Language { get; set; }

        /// <summary>
        /// Universally unique directory identifier.
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// Updated lastly in epoch time.
        /// </summary>
        public int UpdatedAt { get; set; }
        
        /// <summary>
        /// Created at in epoch time.
        /// </summary>
        public int CreatedAt { get; set; }

        /// <summary>
        /// Collection of locations of file systems for this directory
        /// </summary>
        public IEnumerable<Location> Location { get; set; }
    }
}