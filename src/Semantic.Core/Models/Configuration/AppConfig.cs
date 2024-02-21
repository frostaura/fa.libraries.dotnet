using System.Diagnostics;

namespace FrostAura.Libraries.Semantic.Core.Models.Configuration;

/// <summary>
/// Configuration for the App.
/// </summary>
[DebuggerDisplay("{Username}")]
public class AppConfig
{
    /// <summary>
    /// The identifier for the application.
    /// </summary>
    public string Name { get; set; }
}
