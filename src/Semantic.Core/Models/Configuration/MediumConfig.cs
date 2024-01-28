using System.Diagnostics;

namespace FrostAura.Libraries.Semantic.Core.Models.Configuration;

/// <summary>
/// Configuration for Medium blogging platform.
/// </summary>
[DebuggerDisplay("{Token}")]
public class MediumConfig
{
    /// <summary>
    /// Auth token.
    /// </summary>
    public string Token { get; set; }
}
