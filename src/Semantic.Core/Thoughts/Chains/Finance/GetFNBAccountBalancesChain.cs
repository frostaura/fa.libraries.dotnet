using System.ComponentModel;
using FrostAura.Libraries.Semantic.Core.Models.Thoughts;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using FrostAura.Libraries.Semantic.Core.Thoughts.IO;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Cognitive
{
	public class GetFNBAccountBalancesChain : BaseExecutableChain
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
            },
            new Thought
            {
                Action = $"{nameof(OutputThoughts)}.{nameof(OutputThoughts.OutputTextAsync)}",
                Reasoning = "I can simply proxy the response as the LLM already did the formatting for me.",
                Arguments = new Dictionary<string, string>
                {
                    { "output", "$2" }
                },
                OutputKey = "3"
            }
        };

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="serviceProvider">The dependency service provider.</param>
        /// <param name="logger">Instance logger.</param>
        public GetFNBAccountBalancesChain(IServiceProvider serviceProvider, ILogger<GetFNBAccountBalancesChain> logger)
            : base(serviceProvider, logger)
        { }

        /// <summary>
        /// Get FNB account balances as a markdown table.
        /// </summary>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>A markdown table with all accounts and balances.</returns>
        [KernelFunction, Description("Get FNB account balances as a markdown table.")]
        public Task<string> GetFNBAccountBalancesTableAsync(
            CancellationToken token = default)
        {
            return ExecuteChainAsync(string.Empty, token: token);
        }
    }
}
