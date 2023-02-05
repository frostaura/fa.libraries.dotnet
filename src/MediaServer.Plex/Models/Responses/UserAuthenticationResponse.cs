using MediaServer.Plex.Models.Config;

namespace MediaServer.Plex.Models.Responses
{
    /// <summary>
    /// Plex user authentication response.
    /// </summary>
    public class UserAuthenticationResponse
    {
        /// <summary>
        /// Plex user model.
        /// </summary>
        public User User { get; set; }
    }
}