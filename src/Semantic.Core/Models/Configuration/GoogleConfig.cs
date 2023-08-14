using System.Diagnostics;

namespace FrostAura.Libraries.Semantic.Core.Models.Configuration
{
    /// <summary>
    /// Configuration for Google the API.
    /// </summary>
    [DebuggerDisplay("{ApiKey}")]
	public class GoogleConfig
    {
		/// <summary>
		/// The API key to use when connecting to the Google services.
		/// </summary>
		public string ApiKey { get; set; }
    }
}

