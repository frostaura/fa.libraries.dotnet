using Microsoft.Extensions.Logging;

namespace FrostAura.Libraries.Semantic.Core.Data.Logging;

/// <summary>
/// A custom provider for a logger that allows for logging in a tree manner.
/// </summary>
public class HierarchicalLoggerProvider : ILoggerProvider
{
    /// <summary>
    /// Create an instance of ILogger.
    /// </summary>
    /// <param name="categoryName">The calling type name for the logger.</param>
    /// <returns></returns>
    public ILogger CreateLogger(string categoryName)
    {
        return new HierarchicalLogger(categoryName);
    }

    /// <summary>
    /// Clean up unmanaged resources manually.
    /// </summary>
    public void Dispose()
    { }
}
