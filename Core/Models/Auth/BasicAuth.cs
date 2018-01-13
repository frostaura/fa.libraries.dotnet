using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace FrostAura.Libraries.Core.Models.Auth
{
    /// <summary>
    /// Basic auth model.
    /// </summary>
    [DebuggerDisplay("Username: {Username}, Password: {Password}")]
    public class BasicAuth
    {
        /// <summary>
        /// Auth username.
        /// </summary>
        [Required]
        public string Username { get; set; }
        
        /// <summary>
        /// Auth password.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}