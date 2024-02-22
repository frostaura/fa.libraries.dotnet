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
    protected readonly ILogger _logger;

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
}
