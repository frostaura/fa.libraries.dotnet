using System.Collections.Generic;
using System.Diagnostics;

namespace MediaServer.Plex.Models.Content
{
    /// <summary>
    /// Model for media inside of a library.
    /// </summary>
    [DebuggerDisplay("{Title1} - {Title2}")]
    public class MediaContainer
    {
        /// <summary>
        /// Count of media in the container.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Whether or not async is allowed.
        /// </summary>
        public bool AllowSync { get; set; }

        /// <summary>
        /// The poster for the media.
        /// </summary>
        public string Art { get; set; }
        
        /// <summary>
        /// The identifier for the media.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// The section ID for the media.
        /// </summary>
        public int LibrarySectionID { get; set; }
        
        /// <summary>
        /// The section title for the media.
        /// </summary>
        public string LibrarySectionTitle { get; set; }
        
        /// <summary>
        /// The section unique identifier for the media.
        /// </summary>
        public string LibrarySectionUUID { get; set; }
        
        /// <summary>
        /// Media tag prefix of the media.
        /// </summary>
        public string MediaTagPrefix { get; set; }
        
        /// <summary>
        /// Media tag version of the media.
        /// </summary>
        public string MediaTagVersion { get; set; }
        
        /// <summary>
        /// The thumbnail for the media.
        /// </summary>
        public string Thumb { get; set; }
        
        /// <summary>
        /// Title of the media.
        /// </summary>
        public string Title1 { get; set; }
        
        /// <summary>
        /// Secondary title of the media.
        /// </summary>
        public string Title2 { get; set; }
        
        /// <summary>
        /// View group for the media.
        /// </summary>
        public string ViewGroup { get; set; }
        
        /// <summary>
        /// View mode for the media.
        /// </summary>
        public int ViewMode { get; set; }

        /// <summary>
        /// Collection of media content in the media section.
        /// </summary>
        public IEnumerable<Metadata> Metadata { get; set; } = new List<Metadata>();
    }
}