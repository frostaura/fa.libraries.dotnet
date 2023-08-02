using FrostAura.Libraries.Core.Extensions.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Semantic.Core.Attribute.Functions;
using Semantic.Core.Constants.Functions;
using Semantic.Core.Enumerations.Semantic;
using Semantic.Core.Extensions.Functions;
using Semantic.Core.Functions.Composed;
using Semantic.Core.Functions.Semantic;

namespace Semantic.Core.Functions.Procedural.Http
{
    /// <summary>
    /// Take in a URL to perform an HTTP GET request on, extract the text content and summarize it with minimum loss.
    /// </summary>
	public class UrlSummarizer : ComposedFunctionCore
    {
        /// <summary>
        /// The purpose of the function or the kind of solution it provides.
        /// </summary>
        public override string Purpose => $"Take in a URL to perform an HTTP GET request on, extract the text content and summarize it with minimum loss.";
        /// <summary>
        /// The collection of functions to orchestrate in order to solve for X.
        /// </summary>
        protected override IEnumerable<FunctionCore> FunctionsChain => new List<FunctionCore>
        {
            new
        };

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="logger">Instance logger.</param>
        public UrlSummarizer(ILogger logger)
            : base(logger)
        {
        }

        /// <summary>
        /// The purpose of the function or the kind of solution it provides.
        /// </summary>
        /// <param name="arguments">The required arguments provided.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The output string.</returns>
        [Argument(ArgumentNames.INPUT, $"The absolute URL to summarize.")]
        public override Task<string> ExecuteAsync(Dictionary<string, string> arguments, CancellationToken token = default)
        {
            return base.ExecuteAsync(arguments, token);
        }
    }
}
