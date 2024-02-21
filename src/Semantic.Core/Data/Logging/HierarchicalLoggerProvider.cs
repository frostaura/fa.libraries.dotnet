using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Models.Logging;
using Microsoft.Extensions.Logging;

namespace FrostAura.Libraries.Semantic.Core.Data.Logging;

/// <summary>
/// A custom provider for a logger that allows for logging in a tree manner.
/// </summary>
public class HierarchicalLoggerProvider : ILoggerProvider
{
    /// <summary>
    /// A proxy service requirements for a user proxy service.
    /// </summary>
    private readonly IUserProxyDataAccess _userProxy;
    /// <summary>
    /// Local cache for logger instances since we want them to be singletons.
    /// </summary>
    private ILogger _logger;

    /// <summary>
    /// Overloaded constructor to provide dependencies.
    /// </summary>
    /// <param name="userProxyDataAccess">A proxy service requirements for a user proxy service.</param>
    public HierarchicalLoggerProvider(IUserProxyDataAccess userProxyDataAccess)
    {
        _userProxy = userProxyDataAccess
            .ThrowIfNull(nameof(userProxyDataAccess));
    }

    /// <summary>
    /// Create an instance of ILogger.
    /// </summary>
    /// <param name="categoryName">The calling type name for the logger.</param>
    /// <returns>The instance of the logger.</returns>
    public ILogger CreateLogger(string categoryName)
    {
        if (_logger == default)
        {
            Action<List<LogItem>, LogItem> onLogEventHandler = async (scopes, item) => await _userProxy.OnEventAsync(scopes, item);
            _logger = new HierarchicalLogger(onLogEventHandler);
        }

        return _logger;
    }

    /// <summary>
    /// Clean up unmanaged resources manually.
    /// </summary>
    public void Dispose()
    { }
}
