using System.Diagnostics;

namespace FrostAura.Libraries.Semantic.Core.Models.Configuration;

/// <summary>
/// Configuration for IMGBB.
/// </summary>
[DebuggerDisplay("{ApiKey}")]
public class ImgbbConfig
{
    /// <summary>
    /// The API key.
    /// See: https://api.imgbb.com/
    /// </summary>
    public string ApiKey { get; set; }
}
