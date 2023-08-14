﻿using FrostAura.Libraries.Semantic.Core.Models.Thoughts;
using FrostAura.Libraries.Semantic.Core.Thoughts.IO;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using Microsoft.Extensions.Logging;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Cognitive
{
	public class MemoryCommitChain : BaseExecutableChain
	{
        /// <summary>
        /// An example query that this chain example can be used to solve for.
        /// </summary>
        public override string QueryExample => "I was born on the 9th of May 1991.";
        /// <summary>
        /// An example query input that this chain example can be used to solve for.
        /// </summary>
        public override string QueryInputExample => QueryExample;
        /// <summary>
        /// The reasoning for the solution of the chain.
        /// </summary>
        public override string Reasoning => "The birthdate of the user may be valuable in the future. I should commit it to Memory and respond coherently.";
        /// <summary>
        /// A collection of thoughts.
        /// </summary>
        public override List<Thought> ChainOfThoughts => new List<Thought>
        {
            new Thought
            {
                Action = $"{nameof(MemoryThoughts)}.{nameof(MemoryThoughts.CommitToMemoryAsync)}",
                Reasoning = "I will commit the birth date to my Memory for future reference.",
                Critisism = "If I get told the same thing many times, I may record duplicate memories.",
                Arguments = new Dictionary<string, string>
                {
                    { "Memory", "$input" },
                    { "source", "general" }
                },
                OutputKey = "1"
            },
            new Thought
            {
                Action = $"{nameof(OutputThoughts)}.{nameof(OutputThoughts.OutputTextAsync)}",
                Reasoning = "I can respond coherently after saving the Memory.",
                Arguments = new Dictionary<string, string>
                {
                    { "output", "Fantastic! I will remember that for future reference." }
                },
                OutputKey = "2"
            }
        };

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="serviceProvider">The dependency service provider.</param>
        /// <param name="logger">Instance logger.</param>
        public MemoryCommitChain(IServiceProvider serviceProvider, ILogger<MemoryCommitChain> logger)
            : base(serviceProvider, logger)
        { }
    }
}
