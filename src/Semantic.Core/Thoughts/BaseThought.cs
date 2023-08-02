using FrostAura.Libraries.Core.Extensions.Validation;
using Microsoft.Extensions.Logging;

namespace FrostAura.Libraries.Semantic.Core.Thoughts
{
	/// <summary>
	/// The base of all thought containers.
	/// </summary>
	public abstract class BaseThought
	{
        /// <summary>
        /// Instance logger.
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="logger">Instance logger.</param>
        public BaseThought(ILogger logger)
        {
            _logger = logger.ThrowIfNull(nameof(logger));
        }
    }
}
