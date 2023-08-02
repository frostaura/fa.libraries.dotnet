using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Models.Thoughts;
using Microsoft.Extensions.Logging;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Chains
{
	/// <summary>
	/// A decorated collection of a thoughts.
	/// </summary>
	public abstract class BaseExecutableChain
	{
        /// <summary>
        /// An example problem statement that the plan solves for.
        /// </summary>
        public abstract string ExampleChallange { get; }
        /// <summary>
        /// The reasoning for the solution of the chain.
        /// </summary>
        public abstract string Reasoning { get; }
        /// <summary>
        /// A collection of thoughts.
        /// </summary>
        public abstract List<Thought> ChainOfThoughts { get; }
        /// <summary>
        /// Instance logger.
        /// </summary>
        protected readonly ILogger _logger;
        /// <summary>
        /// The dependency service provider.
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="logger">Instance logger.</param>
        public BaseExecutableChain(IServiceProvider serviceProvider, ILogger logger)
        {
            _serviceProvider = serviceProvider.ThrowIfNull(nameof(serviceProvider));
            _logger = logger.ThrowIfNull(nameof(logger));
        }

        /// <summary>
        /// Execute the chain of thought sequentially.
        /// </summary>
        /// <param name="input">The initial input into the chain.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>The chain's final output.</returns>
        public async Task<string> ExecuteChainAsync(string input, CancellationToken token = default)
        {
            var context = new Dictionary<string, string>
            {
                { "$input", input.ThrowIfNullOrWhitespace(nameof(input)) }
            };
            var output = string.Empty;

            foreach (var thought in ChainOfThoughts)
            {
                _logger.LogDebug(thought.ToString());

                thought.Observation = await ExecuteThoughtAsync(thought, context, token);
                output = thought.Observation;
                context[$"${thought.OutputKey}"] = thought.Observation;

                _logger.LogInformation(thought.ToString());
            };

            return output;
        }

        /// <summary>
        /// Execute on a thought and return it's observation.
        /// </summary>
        /// <param name="thought">The thought context.</param>
        /// <param name="context">The global context. Used to interpolate dependencies.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>The thought's response / observation.</returns>
        private Task<string> ExecuteThoughtAsync(Thought thought, Dictionary<string, string> context, CancellationToken token)
        {
            // Interpolate arguments based on the context passed.
            foreach (var argument in thought.Arguments)
            {
                thought.Arguments[argument.Key] = argument.Value;

                foreach (var variable in context)
                {
                    thought.Arguments[argument.Key] = thought.Arguments[argument.Key].Replace(variable.Key, variable.Value);
                }
            }

            // Perform dynamic method execution.
            var thoughName = thought.Action.Split('.')[0];
            var thoughFunctionName = thought.Action.Split('.')[1];
            var thoughtInstance = _serviceProvider.GetThoughtByName(thoughName);
            var methodInfo = thoughtInstance.GetType().GetMethod(thoughFunctionName);
            var parameters = new List<object>();

            foreach (var parameterInfo in methodInfo.GetParameters())
            {
                if (thought.Arguments.ContainsKey(parameterInfo.Name))
                {
                    parameters.Add(Convert.ChangeType(thought.Arguments[parameterInfo.Name], parameterInfo.ParameterType));
                }
                else if (parameterInfo.ParameterType == typeof(CancellationToken))
                {
                    parameters.Add(token);
                }
                else
                {
                    throw new ArgumentException($"Missing argument value for parameter: {parameterInfo.Name}");
                }
            }

            var resultTask = (Task<string>)methodInfo.Invoke(thoughtInstance, parameters.ToArray());

            return resultTask;
        }
    }
}
