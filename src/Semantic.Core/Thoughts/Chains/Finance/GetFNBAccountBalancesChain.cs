using System.ComponentModel;
using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Models.Thoughts;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using FrostAura.Libraries.Semantic.Core.Thoughts.Finance;
using FrostAura.Libraries.Semantic.Core.Thoughts.IO;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using static System.Net.Mime.MediaTypeNames;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Cognitive;

/// <summary>
/// A chain that can get FNB account balances as a markdown table.
/// </summary>
public class GetFNBAccountBalancesChain : BaseChain
{
    /// <summary>
    /// An example query that this chain example can be used to solve for.
    /// </summary>
    public override string QueryExample => "What is my current account balances?";
    /// <summary>
    /// An example query input that this chain example can be used to solve for.
    /// </summary>
    public override string QueryInputExample => string.Empty;
    /// The reasoning for the solution of the chain.
    /// </summary>
    public override string Reasoning => "I can use my some of my thoughts to get the account balances and an LLM to interpret and format the response nicely.";
    /// <summary>
    /// A collection of thoughts.
    /// </summary>
    public override List<Thought> ChainOfThoughts => new List<Thought>
    {
        new Thought
        {
            Action = $"{nameof(FNBThoughts)}.{nameof(FNBThoughts.GetFNBAccountBalancesRawAsync)}",
            Reasoning = "I will use this thought to get the raw account balances as text from FNB.",
            Critisism = "This data is ra HTML inner text so might be difficult to extract the relevant data.",
            Arguments = new Dictionary<string, string>
            { },
            OutputKey = "1"
        },
        new Thought
        {
            Action = $"{nameof(LanguageModelThoughts)}.{nameof(LanguageModelThoughts.PromptLLMAsync)}",
            Reasoning = "I will use an LLM to interpret the raw balances text and extract a coherent, formatted table..",
            Critisism = "I dont know how large the text will be in advance, so will use the large LLM.",
            Arguments = new Dictionary<string, string>
            {
                { "prompt", """
                        Below is a text dump of an HTML page containing bank account balances.
                        Extract the below accounts and balances and give me a neatly-formatted markdown table.

                        Text:
                        $1

                        Table:

                    """ }
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
    public GetFNBAccountBalancesChain(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, ILogger<GetFNBAccountBalancesChain> logger)
        : base(serviceProvider, semanticKernelLanguageModels, logger)
    { }

    /// <summary>
    /// Get FNB account balances as a markdown table.
    /// </summary>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>A markdown table with all accounts and balances.</returns>
    [KernelFunction, Description("Get First National Bank accounts balances formatted as a markdown table.")]
    public Task<string> GetFNBAccountBalancesTableAsync(
        CancellationToken token = default)
    {
        using (_logger.BeginScope("{MethodName}", nameof(GetFNBAccountBalancesTableAsync)))
        {
            return ExecuteChainAsync(string.Empty, token: token);
        }
    }
}

/// <summary>
/// Mermaid diagram for the GetFNBAccountBalancesChain class.
/// </summary>
/// <remarks>
/// This diagram provides a visual representation of the GetFNBAccountBalancesChain class, its methods, and their interactions.
/// </remarks>
/// <code>
/// classDiagram
///     class GetFNBAccountBalancesChain {
///         +GetFNBAccountBalancesChain(IServiceProvider, ISemanticKernelLanguageModelsDataAccess, ILogger<GetFNBAccountBalancesChain>)
///         +Task~string~ GetFNBAccountBalancesTableAsync(CancellationToken)
///     }
///     GetFNBAccountBalancesChain --> BaseChain
///     BaseChain <|-- GetFNBAccountBalancesChain
///     GetFNBAccountBalancesChain : +string QueryExample
///     GetFNBAccountBalancesChain : +string QueryInputExample
///     GetFNBAccountBalancesChain : +string Reasoning
///     GetFNBAccountBalancesChain : +List~Thought~ ChainOfThoughts
/// </code>
