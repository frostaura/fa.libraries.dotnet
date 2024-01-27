using Microsoft.Extensions.Logging;

namespace FrostAura.Libraries.Semantic.Core.Data.Logging;

/// <summary>
/// A custom logger that allows for status updates.
/// </summary>
public class ProgressiveLoggerProviderDataAccess : ILoggerProvider
{

    public ILogger CreateLogger(string categoryName)
    {
        return new ProgressiveLoggerDataAccess(categoryName);
    }

    public void Dispose()
    { }
}
