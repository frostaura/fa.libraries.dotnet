using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace MediaServer.Plex.Models.Config
{
    /// <summary>
    /// Plex specific media server configuration.
    /// </summary>
    [DebuggerDisplay("{ServerAddress} - {PlexToken}")]
    public class PlexMediaServerConfig
    {
        /// <summary>
        /// Token used in web requests to the media server for authentication.
        /// </summary>
        [Required]
        public string PlexToken { get; set; }
        
        /// <summary>
        /// The fully qualified address of the Plex media server.
        /// E.g. http://192.168.0.5:32400/
        /// </summary>
        public string ServerAddress { get; set; }
        
        /// <summary>
        /// Where Plex server preferences get stored.
        /// </summary>
        public ServerPreferences ServerPreferences { get; set; }

        /// <summary>
        /// Token appendable to the query string.
        /// </summary>
        public string QueryStringPlexToken => $"X-Plex-Token={PlexToken}";
    }
}