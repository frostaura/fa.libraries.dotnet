using System.Diagnostics;

namespace MediaServer.Plex.Models.Content
{
    /// <summary>
    /// File system location for items.
    /// </summary>
    [DebuggerDisplay("Path: {Path}")]
    public class Location
    {
        /// <summary>
        /// Unique identifier for the location.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Fully qualified file system path of the location.
        /// </summary>
        public string Path { get; set; }
    }
}