using System.Diagnostics;
using FrostAura.Libraries.Semantic.Core.Enums.Logging;
using Newtonsoft.Json;

namespace FrostAura.Libraries.Semantic.Core.Models.Logging;

/// <summary>
/// A log item object
/// </summary>
[DebuggerDisplay("[{Type}]: {Message}")]
public class LogItem
{
    /// <summary>
    /// The unique oepration ID that this particular log item is for/under.
    /// </summary>
    public int ScopeOperationId { get; set; }
    /// <summary>
    /// The type of the log item.
    /// </summary>
    public LogType Type { get; set; }
    /// <summary>
    /// The item message.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Override how the object is represented as string.
    /// </summary>
    /// <returns>A JSON serialized string representation of the log item.</returns>
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
