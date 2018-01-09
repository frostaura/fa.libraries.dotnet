using System.Diagnostics;

namespace MediaServer.Plex.Models.Content
{
    /// <summary>
    /// Genre model of media.
    /// </summary>
    [DebuggerDisplay("{Tag}")]
    public class TagWithValue
    {
        /// <summary>
        /// Tag of the genre.
        /// </summary>
        public string Tag { get; set; }
    }
}