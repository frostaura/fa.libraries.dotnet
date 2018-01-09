using System.Collections.Generic;
using System.Diagnostics;
using MediaServer.Plex.Models.Content;

namespace MediaServer.Plex.Models.Collections
{
    /// <summary>
    /// Model for get all libraries response.
    /// </summary>
    [DebuggerDisplay("Id: {Identifier}, Count: {Size}")]
    public class Libraries
    {
        /// <summary>
        /// Length of directories found.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Whether or not async is allowed.
        /// </summary>
        public bool AllowAsync { get; set; }

        /// <summary>
        /// Unique identifier of type library.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Root to media tags.
        /// </summary>
        public string MediaTagPrefix { get; set; }

        /// <summary>
        /// Version of media tag system.
        /// </summary>
        public string MediaTagVersion { get; set; }

        /// <summary>
        /// Master collection name.
        /// </summary>
        public string Title1 { get; set; }

        /// <summary>
        /// Collection of libraries found in master collection.
        /// </summary>
        public IEnumerable<Directory> Directory { get; set; }
    }
}