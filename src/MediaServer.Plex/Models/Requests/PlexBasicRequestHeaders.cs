namespace MediaServer.Plex.Models.Requests
{
    public class PlexBasicRequestHeaders
    {
        /// <summary>
        /// Platform the player runs on.
        /// </summary>
        public string Platform { get; set; } = "Windows";
        
        /// <summary>
        /// Platform version the player runs on.
        /// </summary>
        public string PlatformVersion { get; set; } = "NT";

        /// <summary>
        /// Specify that the comsuming service is in fact a player.
        /// </summary>
        public string Provides { get; set; } = "player";

        /// <summary>
        /// Unique client identifier of the player.
        /// </summary>
        public string ClientIdentifier { get; set; } = "FrostAuraPlexPlayer";

        /// <summary>
        /// Plex product specification.
        /// </summary>
        public string Product { get; set; } = "PlexWMC";

        /// <summary>
        /// Plex product version.
        /// </summary>
        public string Version { get; set; } = "0";
    }
}