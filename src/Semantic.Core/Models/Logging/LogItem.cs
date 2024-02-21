using System.Diagnostics;
using FrostAura.Libraries.Semantic.Core.Enums.Logging;
using Newtonsoft.Json;

namespace FrostAura.Libraries.Semantic.Core.Models.Logging;

/// <summary>
/// A log item object
/// </summary>
[DebuggerDisplay("[{Type} ({Status})]: {Message}")]
public class LogItem
{
    /// <summary>
    /// The unique identifier.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    /// <summary>
    /// The unique parent.
    /// </summary>
    [JsonIgnore]
    public LogItem? Scope { get; set; }
    /// <summary>
    /// The type of the log item.
    /// </summary>
    public LogType Type { get; set; }
    /// <summary>
    /// The item message.
    /// </summary>
    public required string Message { get; set; }
    /// <summary>
    /// The current status of the log item.
    /// </summary>
    public LogStatus Status { get; set; }
    /// <summary>
    /// A collection of attributes to associate with the log item.
    /// </summary>
    public readonly Dictionary<string, object> Attributes = new Dictionary<string, object>();
    /// <summary>
    /// Child log items.
    /// </summary>
    public readonly List<LogItem> Logs = new List<LogItem>();

    /// <summary>
    /// Override how the object is represented as string.
    /// </summary>
    /// <returns>A JSON serialized string representation of the log item.</returns>
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
