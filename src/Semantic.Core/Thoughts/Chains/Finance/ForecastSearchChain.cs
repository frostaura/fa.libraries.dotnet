using System.ComponentModel;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Models.Thoughts;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using FrostAura.Libraries.Semantic.Core.Thoughts.IO;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Finance;

/// <summary>
/// A chain that can search the internet for relevant and trusted sources of forecasts for a given asset and timeframe.
/// </summary>
public class ForecastSearchChain : BaseChain
{
    /// <summary>
    /// An example query that this chain example can be used to solve for.
    /// </summary>
    public override string QueryExample => "Get the forecast for Bitcoin for today.";
    /// <summary>
    /// An example query input that this chain example can be used to solve for.
    /// </summary>
    public override string QueryInputExample => "Bitcoin for today";
    /// The reasoning for the solution of the chain.
    /// </summary>
    public override string Reasoning => "I can use my web driver to search the internet for relevant and trusted sources of forecasts for the given asset and timeframe, fetch the forecast data, weigh them based on the reputation of the source, and respond with a forecast.";
    /// <summary>
    /// A collection of thoughts.
    /// </summary>
    public override List<Thought> ChainOfThoughts => new List<Thought>
    {
        new Thought
        {
            Action = $"{nameof(WebDriverThoughts)}.{nameof(WebDriverThoughts.SearchForecastsAsync)}",
            Reasoning = "I will use my web driver to search the internet for relevant and trusted sources of forecasts for the given asset and timeframe.",
            Critisism = "I need to ensure that I use trusted sources for the forecasts.",
            Arguments = new Dictionary<string, string>
            {
                { "assetName", "$input" },
                { "timeframe", "$input" }
            },
            OutputKey = "1"
        },
        new Thought
        {
            Action = $"{nameof(LanguageModelThoughts)}.{nameof(LanguageModelThoughts.WeighForecastsAsync)}",
            Reasoning = "I will use a language model to weigh the forecasts based on the reputation of the source.",
            Critisism = "I need to ensure that I use the correct reputation data for the sources.",
            Arguments = new Dictionary<string, string>
            {
                { "forecasts", "$1" }
            },
            OutputKey = "2"
        }
    };

    /// <summary>
    /// Overloaded constructor to provide dependencies.
    /// </summary>
    /// <param name="serviceProvider">The dependency service provider.</param>
    /// <param name="semanticKernelLanguageModels">A component for communicating with language models.</param>
    /// <param name="logger">Instance logger.</param>
    public ForecastSearchChain(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, ILogger<ForecastSearchChain> logger)
        : base(serviceProvider, semanticKernelLanguageModels, logger)
    { }

    /// <summary>
    /// Get the forecast for a given asset and timeframe.
    /// </summary>
    /// <param name="assetName">The name or ticker of the asset. For example, BTC or Bitcoin.</param>
    /// <param name="timeframe">The timeframe for the forecast. For example, "for today", "for tomorrow" or "in 6 months".</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The forecast for the given asset and timeframe.</returns>
    [KernelFunction, Description("Get the forecast for a given asset and timeframe.")]
    public Task<string> GetForecastAsync(
        [Description("The name or ticker of the asset. For example, BTC or Bitcoin.")] string assetName,
        [Description("The timeframe for the forecast. For example, 'for today', 'for tomorrow' or 'in 6 months'.")] string timeframe,
        CancellationToken token = default)
    {
        using (_logger.BeginScope("{MethodName}", nameof(GetForecastAsync)))
        {
            _logger.LogInformation("Starting forecast search for {AssetName} {Timeframe}", assetName, timeframe);
            return ExecuteChainAsync($"{assetName} {timeframe}".ThrowIfNullOrWhitespace(nameof(assetName)), token: token);
        }
    }
}
