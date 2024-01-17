using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Chains.PlanningThoughts;

/// <summary>
/// Planning thoughts.
/// </summary>
public class PlanningThought : BaseThought
{
    /// <summary>
    /// Overloaded constructor to provide dependencies.
    /// </summary>
    /// <param name="serviceProvider">The dependency service provider.</param>
    /// <param name="semanticKernelLanguageModels">A component for communicating with language models.</param>
    /// <param name="logger">Instance logger.</param>
    public PlanningThought(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, ILogger<PlanningThought> logger)
        : base(serviceProvider, semanticKernelLanguageModels, logger)
    { }

    /// <summary>
    /// Solve for a query step-by-step, by using available thoughts.
    /// </summary>
    /// <param name="query">The query to solve for.</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The response body as a string.</returns>
    [KernelFunction, Description("Solve for a complex query or problem step-by-step. Can be used as a last resort to solve a problem when no other answer is known.")]
    public async Task<string> SolveWithExamplesAsync(
        [Description("The query to solve for.")] string query,
        CancellationToken token = default)
    {
        const string IDENTITY_PARAM_KEY = "identity";
        const string IDENTITY = "You are Zeus, the Greek god, a powerful and intelligent task planner.";
        var llmThought = (LanguageModelThoughts)_serviceProvider
            .GetThoughtByName(nameof(LanguageModelThoughts));
        var prompt = string.Empty
            .Replace($"${IDENTITY_PARAM_KEY}$", IDENTITY);

        throw new NotImplementedException("Load prompt from file.");

        var planString = await llmThought.PromptLLMAsync(prompt, token);
    }
}
