using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Models.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FrostAura.Libraries.Semantic.Core.Data.Logging;

/// <summary>
/// A custom logger that allows for status updates.
/// </summary>
[DebuggerDisplay("{_callingTypeName}")]
public class HierarchicalLogger : ILogger
{
    /// <summary>
    /// The full type name of the calling object.
    /// </summary>
    private readonly string _callingTypeName;
    /// <summary>
    /// Scope information.
    /// </summary>
    private readonly ConcurrentDictionary<int, List<string>> _scopeInformation;

    /// <summary>
    /// Overloaded constructor to provide depndencies.
    /// </summary>
    /// <param name="callingTypeName">The full type name of the calling object.</param>
    public HierarchicalLogger(string callingTypeName)
    {
        _callingTypeName = callingTypeName
            .ThrowIfNullOrWhitespace(nameof(callingTypeName));
        _scopeInformation = new ConcurrentDictionary<int, List<string>>();
    }

    /// <summary>
    /// Determiner for whether this logger is enabled.
    /// </summary>
    /// <param name="logLevel">Log level / importance.</param>
    /// <returns>Whether this logger should be enabled.</returns>
    public bool IsEnabled(LogLevel logLevel) => true;

    /// <summary>
    /// Begin a logging scope.
    /// </summary>
    /// <typeparam name="TState">The input state for the scope's type.</typeparam>
    /// <param name="state">The input state for the scope.</param>
    /// <returns>A disposable scope.</returns>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        if (state is KeyValuePair<string, object> scopeKeyValue &&
            scopeKeyValue.Key is string scopeName &&
            scopeKeyValue.Value is object callerInstance)
        {
            var operationId = RuntimeHelpers.GetHashCode(callerInstance);

            _scopeInformation.TryAdd(operationId, new List<string> { scopeName });

            return new HierarchicalScope(operationId, _scopeInformation);
        }

        return null;
    }

    /// <summary>
    /// Log a message.
    /// </summary>
    /// <typeparam name="TState">Incoming message state type.</typeparam>
    /// <param name="logLevel">Log level / importance.</param>
    /// <param name="eventId">Log event identifier.</param>
    /// <param name="state">Incoming message state.</param>
    /// <param name="exception">Exception object, if any.</param>
    /// <param name="formatter">Formatter to generate the final output.</param>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);

        try
        {
            var parsedMessage = JsonConvert.DeserializeObject<LogItem>(message);

            if (_scopeInformation.TryGetValue(parsedMessage.ScopeOperationId, out var scopes))
            {
                // You can now access information from nested scopes
                foreach (var scope in scopes)
                {
                    Console.WriteLine($"Scope: {scope}, Message: {parsedMessage.Message}");
                    // TODO: LogState. For now just 2 levels but supporting nested levels.
                    // TODO: Go through every thought and add info and debug logs as well as scopes.
                }
            }
        }
        catch (Exception)
        {
            var consoleColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine($"Failed to parse error '{message}' as type '{nameof(LogItem)}'.");

            Console.ForegroundColor = consoleColor;
        }
    }
}
