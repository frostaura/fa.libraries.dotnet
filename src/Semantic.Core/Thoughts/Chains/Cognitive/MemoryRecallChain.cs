using FrostAura.Libraries.Semantic.Core.Models.Thoughts;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using FrostAura.Libraries.Semantic.Core.Thoughts.IO;
using Microsoft.Extensions.Logging;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Cognitive
{
	public class MemoryRecallChain : BaseExecutableChain
	{
        /// <summary>
        /// The problem statement that the plan solves for.
        /// </summary>
        public override string QueryExample => "When was I born?";
        /// <summary>
        /// An example query input that this chain example can be used to solve for.
        /// </summary>
        public override string QueryInputExample => QueryExample;
        /// <summary>
        /// The reasoning for the solution of the chain.
        /// </summary>
        public override string Reasoning => "This seems like information that I may have committed to my Memory before. I can look this up from Memory and respond coherently using a LLM. If the information doesn't exist in my Memory, I should say I don't know.";
        /// <summary>
        /// A collection of thoughts.
        /// </summary>
        public override List<Thought> ChainOfThoughts => new List<Thought>
        {
            new Thought
            {
                Action = $"{nameof(MemoryThoughts)}.{nameof(MemoryThoughts.RecallFromMemoryAsync)}",
                Reasoning = "I will look this information up from my Memory store.",
                Critisism = "When memories aren't available, irrelevant memories may be recalled. I should only consider the relevant one(s).",
                Arguments = new Dictionary<string, string>
                {
                    { "query", "$input" }
                },
                OutputKey = "1"
            },
            new Thought
            {
                Action = $"{nameof(LanguageModelThoughts)}.{nameof(LanguageModelThoughts.PromptSmallLLMAsync)}",
                Reasoning = "I will pass the received memories into a LLM to get a coherent response dynamically. I will build a prompt that I believe may work well to achieve this. I will use a small LLM because I prefer speed over reasoning for this task.",
                Critisism = "There may be a better ways to form my prompt. I should also make my prompts concise as the context window size of an LLM is limited.",
                Arguments = new Dictionary<string, string>
                {
                    { "prompt", "Given the following memories from a knowledge base, answer the following question. If the memories are not relevant or you don't know, say so.+++MEMORIES: $1+++QUESTION: $input+++ANSWER:" }
                },
                OutputKey = "2"
            },
            new Thought
            {
                Action = $"{nameof(OutputThoughts)}.{nameof(OutputThoughts.OutputTextAsync)}",
                Reasoning = "I can simply proxy the LLM response as I expect it to be coherent and informative.",
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
        public MemoryRecallChain(IServiceProvider serviceProvider, ILogger<MemoryRecallChain> logger)
            : base(serviceProvider, logger)
        { }
    }
}
