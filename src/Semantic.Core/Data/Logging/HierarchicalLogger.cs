using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Data.Adapters;
using FrostAura.Libraries.Semantic.Core.Enums.Logging;
using FrostAura.Libraries.Semantic.Core.Models.Logging;
using FrostAura.Libraries.Semantic.Core.Models.Prompts;
using Microsoft.Extensions.Logging;

namespace FrostAura.Libraries.Semantic.Core.Data.Logging;

/// <summary>
/// A custom logger that allows for status updates.
/// </summary>
public class HierarchicalLogger : ILogger
{
    /// <summary>
    /// The current semantic operation context.
    /// </summary>
    public static AsyncLocal<OperationContext> CurrentSemanticOperationContext = new AsyncLocal<OperationContext>();
    /// <summary>
    /// Scopes collection.
    /// </summary>
    private readonly List<LogItem> _scopes;
    /// <summary>
    /// Handler for when logs occur.
    /// </summary>
    private readonly Action<List<LogItem>, LogItem> _onEventHandler;

    /// <summary>
    /// Overloaded constructor to provide depndencies.
    /// </summary>
    /// <param name="onEventHandler">Handler for when logs occur.</param>
    public HierarchicalLogger(Action<List<LogItem>, LogItem> onEventHandler)
    {
        _scopes = new List<LogItem>();
        _onEventHandler = onEventHandler
            .ThrowIfNull(nameof(onEventHandler));
    }

    /// <summary>
    /// Begin a logging scope.
    /// </summary>
    /// <typeparam name="TState">The input state for the scope's type.</typeparam>
    /// <param name="state">The input state for the scope.</param>
    /// <returns>A disposable scope.</returns>
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        var scopeLogItem = new LogItem
        {
            Message = state.ToString(),
            Type = LogType.ScopeRoot,
            Status = LogStatus.Busy
        };

        // Determine the parent of the item, if any.
        var activeScopesDesc = _scopes
            .Where(s => s.Status == LogStatus.Busy && s.Type == LogType.ScopeRoot)
            .Reverse()
            .FirstOrDefault();

        if (activeScopesDesc != default)
        {
            scopeLogItem.Scope = activeScopesDesc;
        }

        _scopes.Add(scopeLogItem);

        return new DisposableAdapter(() => scopeLogItem.Status = LogStatus.Succeeded);
    }

    /// <summary>
    /// Whether the logger is enabled.
    /// </summary>
    /// <param name="logLevel">Current log level.</param>
    /// <returns>Whether the logger is enabled.</returns>
    public bool IsEnabled(LogLevel logLevel) => true;

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
        try
        {
            // Determine log item's parent scope.
            var activeScopesLatestFirst = _scopes
                .Where(s => s.Status == LogStatus.Busy && s.Type == LogType.ScopeRoot)
                .Reverse();
            var activeScope = activeScopesLatestFirst.FirstOrDefault();
            var parsedMessage = new LogItem
            {
                Scope = activeScope,
                Status = LogStatus.Busy,
                Message = state.ToString(),
                OperationContext = CurrentSemanticOperationContext.Value
            };

            activeScope?.Logs.ForEach(l => l.Status = LogStatus.Succeeded);
            activeScope?.Logs.Add(parsedMessage);
            _onEventHandler.Invoke(_scopes, parsedMessage);
        }
        catch (Exception ex)
        { /* Swallow non-semantic errors. */ }
    }
}
