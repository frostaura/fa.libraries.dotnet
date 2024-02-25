using System.Diagnostics;

namespace FrostAura.Libraries.Semantic.Core.Models.Prompts;

/// <summary>
/// Semantic request operation context.
/// </summary>
[DebuggerDisplay("[{Id}] {UserIdentifier}")]
public class OperationContext
{
    /// <summary>
    /// Unique operation id.
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// The display name of the context.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Additional attributes to pass along the context.
    /// </summary>
    public Dictionary<string, object> Attributes { get; private set; } = new Dictionary<string, object>();

}
