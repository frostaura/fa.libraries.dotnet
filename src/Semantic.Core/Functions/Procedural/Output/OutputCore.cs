using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Semantic.Core.Attribute.Functions;
using Semantic.Core.Constants.Functions;
using Semantic.Core.Enumerations.Semantic;
using Semantic.Core.Extensions.Functions;
using Semantic.Core.Functions.Semantic;

namespace Semantic.Core.Functions.Procedural
{
    /// <summary>
    /// Produce a core output.
    /// </summary>
	public abstract class OutputCore : FunctionCore
    {
        /// <summary>
        /// The type of the content of the output.
        /// </summary>
        public abstract string Type { get; }
        /// <summary>
        /// The purpose of the function or the kind of solution it provides.
        /// </summary>
        public override string Purpose => $"Allows for feeding back to the system user with {Type} content. This can be used many times as well as mixed with other outputs.";
        /// <summary>
        /// The content of the input to allow for extraction.
        /// </summary>
        public string Content { get; private set; } = default;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="logger">Instance logger.</param>
        public OutputCore(ILogger logger)
            : base(logger)
        { }

        /// <summary>
        /// The purpose of the function or the kind of solution it provides.
        /// </summary>
        /// <param name="arguments">The required arguments provided.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The output string.</returns>
        [Argument(ArgumentNames.OUTPUT, $"The content of the output.")]
        public override Task<string> ExecuteAsync(Dictionary<string, string> arguments, CancellationToken token = default)
        {
            Content = arguments.GetArgument(ArgumentNames.OUTPUT);

            return Task.FromResult(Content);
        }
    }
}

