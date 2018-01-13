using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using FrostAura.Libraries.Core.Models.Auth;
using MediaServer.Plex.Models.Requests;

namespace MediaServer.Plex.Models.Config
{
    /// <summary>
    /// Plex specific media server configuration.
    /// </summary>
    [DebuggerDisplay("{ServerAddress}")]
    public class PlexMediaServerConfig
    {
        /// <summary>
        /// User context used in web requests to the media server for authorization.
        /// </summary>
        public User PlexAuthenticatedUser { get; set; }
        
        /// <summary>
        /// User details to make an authentication request with in order to get an authorization token.
        /// </summary>
        [Required]
        public BasicAuth PlexAuthenticationRequestUser { get; set; }
        
        /// <summary>
        /// The fully qualified address of the Plex media server.
        /// E.g. http://192.168.0.5:32400/
        /// </summary>
        [Required]
        public string ServerAddress { get; set; }
        
        /// <summary>
        /// Where Plex server preferences get stored.
        /// </summary>
        public ServerPreferences ServerPreferences { get; set; }

        /// <summary>
        /// Token appendable to the query string.
        /// </summary>
        public string QueryStringPlexToken => $"X-Plex-Token={PlexAuthenticatedUser.AuthToken}";

        /// <summary>
        /// Basic Plex request headers.
        /// </summary>
        public PlexBasicRequestHeaders BasicPlexHeaders { get; set; }
    }
}