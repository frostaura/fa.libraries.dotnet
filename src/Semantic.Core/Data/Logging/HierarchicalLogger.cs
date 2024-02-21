using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Data.Adapters;
using FrostAura.Libraries.Semantic.Core.Enums.Logging;
using FrostAura.Libraries.Semantic.Core.Models.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace FrostAura.Libraries.Semantic.Core.Data.Logging;

/// <summary>
/// A custom logger that allows for status updates.
/// </summary>
public class HierarchicalLogger : ILogger
{
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
            var scopeLogItem = new LogItem
            {
                Message = scopeName,
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

            scopeLogItem.Attributes.Add("CallerInstance", callerInstance);
            _scopes.Add(scopeLogItem);

            return new DisposableAdapter(() => scopeLogItem.Status = LogStatus.Succeeded);
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

            if (parsedMessage == default) return;

            // Determine log item's parent scope.
            var activeScopesLatestFirst = _scopes
                .Where(s => s.Status == LogStatus.Busy && s.Type == LogType.ScopeRoot)
                .Reverse();
            var activeScope = activeScopesLatestFirst.FirstOrDefault();
            parsedMessage.Scope = activeScope;
            parsedMessage.Status = LogStatus.Busy;

            activeScope?.Logs.ForEach(l => l.Status = LogStatus.Succeeded);
            activeScope?.Logs.Add(parsedMessage);
            _onEventHandler.Invoke(_scopes, parsedMessage);
        }
        catch (Exception e)
        { /* Ignore non-semantic errors. */ }
    }
}
