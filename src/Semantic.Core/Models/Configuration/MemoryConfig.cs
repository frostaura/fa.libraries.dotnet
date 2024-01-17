using System.Diagnostics;

namespace FrostAura.Libraries.Semantic.Core.Models.Configuration;

/// <summary>
/// Configuration for semantic memory.
/// </summary>
[DebuggerDisplay("{CollectionName}: Top {TopK}")]
public class SemanticMemoryConfig
{
    /// <summary>
    /// The collection to use for Memory storage.
    /// </summary>
    public string CollectionName { get; set; } = $"{Guid.NewGuid()}";
    /// <summary>
    /// The count of top memories to include from a search.
    /// </summary>
    public int TopK { get; set; } = 5;
}
