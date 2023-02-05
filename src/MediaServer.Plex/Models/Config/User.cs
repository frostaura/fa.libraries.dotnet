using System;

namespace MediaServer.Plex.Models.Config
{
    /// <summary>
    /// Plex user model.
    /// </summary>
    public class User
    {
        /// <summary>
        /// User identifier.
        /// </summary>
        public Int64 Id { get; set; }

        /// <summary>
        /// Universally unique user identifier.
        /// </summary>
        public string Uuid { get; set; }

        /// <summary>
        /// User emaial address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Chosen username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// User title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// User full thumbnail image URL.
        /// </summary>
        public string Thumb { get; set; }

        /// <summary>
        /// Whether or not the user has a password.
        /// </summary>
        public bool HasPassword { get; set; }

        /// <summary>
        /// Authentication token.
        /// </summary>
        public string AuthToken { get; set; }
    }
}