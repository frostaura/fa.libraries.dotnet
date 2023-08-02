using System.ComponentModel;
using FrostAura.Libraries.Core.Extensions.Validation;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.SkillDefinition;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.IO
{
    /// <summary>
    /// Http thoughts.
    /// </summary>
    public class OutputThoughts : BaseThought
    {
        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="logger">Instance logger.</param>
        public OutputThoughts(ILogger<HttpThoughts> logger)
            : base(logger)
        { }

        /// <summary>
        /// Output text conetnt to the end-user.
        /// </summary>
        /// <param name="uri">URI of the request.</param>
        /// <param name="cancellationToken">The token to use to request cancellation.</param>
        /// <returns>The response body as a string.</returns>
        [SKFunction, Description("Output text content to the end-user.")]
        public Task<string> OutputTextAsync(
            [Description("The text to output.")] string output,
            CancellationToken token = default)
        {
            return Task.FromResult(output.ThrowIfNullOrWhitespace(nameof(output)));
        }
    }
}