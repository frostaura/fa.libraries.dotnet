using System.Diagnostics;

namespace FrostAura.Libraries.Semantic.Core.Models.Configuration
{
	/// <summary>
	/// Configuration for FNB.
	/// </summary>
	[DebuggerDisplay("{Username}")]
	public class FNBConfig
	{
        /// <summary>
        /// FNB username.
        /// </summary>
        public string Username { get; set; }
		/// <summary>
		/// FNB password.
		/// </summary>
		public string Password { get; set; }
    }
}

