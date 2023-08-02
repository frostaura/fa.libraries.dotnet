using System.Diagnostics;

namespace FrostAura.Libraries.Semantic.Core.Models.Configuration
{
    /// <summary>
    /// Configuration for Pinecone vector db.
    /// </summary>
	[DebuggerDisplay("{Environment}")]
    public class PineconeConfig
    {
		/// <summary>
		/// The environment / hosting location.
		/// </summary>
		public string Environment { get; set; }
		/// <summary>
		/// The API key for security.
		/// </summary>
		public string ApiKey { get; set; }
    }
}

