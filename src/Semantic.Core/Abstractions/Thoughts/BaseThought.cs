using System.Runtime.CompilerServices;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using Microsoft.Extensions.Logging;

namespace FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;

/// <summary>
/// The base of all thought containers.
/// </summary>
public abstract class BaseThought
{
    /// <summary>
    /// The dependency service provider.
    /// </summary>
    protected readonly IServiceProvider _serviceProvider;
    /// <summary>
    /// A component for communicating with language models.
    /// </summary>
    protected readonly ISemanticKernelLanguageModelsDataAccess _semanticKernelLanguageModels;
    /// <summary>
    /// Instance logger.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    /// Overloaded constructor to provide dependencies.
    /// </summary>
    /// <param name="serviceProvider">The dependency service provider.</param>
    /// <param name="semanticKernelLanguageModels">A component for communicating with language models.</param>
    /// <param name="logger">Instance logger.</param>
    public BaseThought(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, ILogger logger)
    {
        _serviceProvider = serviceProvider.ThrowIfNull(nameof(serviceProvider));
        _semanticKernelLanguageModels = semanticKernelLanguageModels.ThrowIfNull(nameof(semanticKernelLanguageModels));
        _logger = logger.ThrowIfNull(nameof(logger));
    }

    /// <summary>
    /// Create a baselined disposable logging scope.
    /// </summary>
    /// <param name="scopeDescription">The main items grouping text.</param>
    /// <returns>A disposable logging scope.</returns>
    protected IDisposable BeginSemanticScope(string scopeDescription)
    {
        return _logger.BeginScope(new KeyValuePair<string, object>(scopeDescription, this));
    }

    /// <summary>
    /// Log information with additional semantic data decorations.
    /// </summary>
    /// <param name="message">The message to log.</param>
    protected void LogSemanticInformation(string message)
    {
        var operationId = RuntimeHelpers.GetHashCode(this);

        _logger.LogInformation($"[[[{operationId}]]][[[INFO]]]{message}");
    }

    /// <summary>
    /// Log debug with additional semantic data decorations.
    /// </summary>
    /// <param name="message">The message to log.</param>
    protected void LogSemanticDebug(string message)
    {
        var operationId = RuntimeHelpers.GetHashCode(this);

        _logger.LogDebug($"[[[{operationId}]]][[[DEBUG]]]{message}");
    }

    /// <summary>
    /// Log error with additional semantic data decorations.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="exception">Exception instance.</param>
    protected void LogSemanticError(string message, Exception exception)
    {
        var operationId = RuntimeHelpers.GetHashCode(this);

        _logger.LogError($"[[[{operationId}]]][[[ERROR]]]{message}", exception);
    }
}
