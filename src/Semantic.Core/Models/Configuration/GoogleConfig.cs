using System.Diagnostics;

namespace FrostAura.Libraries.Semantic.Core.Models.Configuration;

/// <summary>
/// Configuration for Google the API.
/// </summary>
[DebuggerDisplay("{ApiKey}")]
public class GoogleConfig
{
	/// <summary>
	/// The OAuth concent token to use.
	/// </summary>
	public string OAuthToken { get; set; }
}
