using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Models.Thoughts;
using Microsoft.Extensions.Logging;

namespace FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;

/// <summary>
/// A decorated collection of a thoughts.
/// </summary>
public abstract class BaseChain : BaseThought
{
    /// <summary>
    /// An example query that this chain can be used to solve for.
    ///
    /// Example: "Transcribe the file '/absolute/path/example.mp3' and return the transcription text."
    /// </summary>
    public abstract string QueryExample { get; }
    /// <summary>
    /// An example query input that this chain example can be used to solve for. The value that should be 
    ///
    /// Example: "/absolute/path/example.mp3"
    /// </summary>
    public abstract string QueryInputExample { get; }
    /// <summary>
    /// The reasoning for the solution of the chain.
    ///
    /// Example: "I can use my code interpreter to create a script to use the OpenAI Whisper model to transcribe an Audio file at a specific path and return the text transcription."
    /// </summary>
    public abstract string Reasoning { get; }
    /// <summary>
    /// A collection of thoughts / steps that should be executed to solve for the input query.
    /// </summary>
    public abstract List<Thought> ChainOfThoughts { get; }

    /// <summary>
    /// Overloaded constructor to provide dependencies.
    /// </summary>
    /// <param name="serviceProvider">The dependency service provider.</param>
    /// <param name="semanticKernelLanguageModels">A component for communicating with language models.</param>
    /// <param name="logger">Instance logger.</param>
    protected BaseChain(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, ILogger logger)
        :base(serviceProvider, semanticKernelLanguageModels, logger)
    { }

    /// <summary>
    /// Execute the chain of thought sequentially. This function is meant to be called by other functions on the derrived object.
    /// </summary>
    /// <param name="input">The initial input into the chain.</param>
    /// <param name="state">The optional state of the chain. Should state be provided for outputs, thoughts that produce such outputs would be skipped.</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The chain's final output.</returns>
    protected virtual async Task<string> ExecuteChainAsync(string input = "", Dictionary<string, string> state = default, CancellationToken token = default)
    {
        using (_logger.BeginScope("{MethodName} with input: {Input}", nameof(ExecuteChainAsync), input))
        {
            if (state == default) state = new Dictionary<string, string>();

            state["$input"] = input;

            var output = string.Empty;

            foreach (var thought in ChainOfThoughts)
            {
                _logger.LogDebug("Executing {Thought} in the chain.", thought);

                // If there is already the state that this thought is expected to provide, use that state and skip executing the thought.
                if (state.ContainsKey($"${thought.OutputKey}"))
                {
                    thought.Observation = state[$"${thought.OutputKey}"];
                }
                // The state should be created by executing the thought.
                else
                {
                    thought.Observation = await ExecuteThoughtAsync(thought, state, token);
                    state[$"${thought.OutputKey}"] = thought.Observation;
                }

                output = thought.Observation;

                _logger.LogInformation("{Thought}", thought);
            };

            return output;
        }
    }

    /// <summary>
    /// Execute on a thought and return it's observation.
    /// </summary>
    /// <param name="thought">The thought context.</param>
    /// <param name="state">The global context. Used to interpolate dependencies.</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The thought's response / observation.</returns>
    protected Task<string> ExecuteThoughtAsync(Thought thought, Dictionary<string, string> state, CancellationToken token)
    {
        using (_logger.BeginScope("{MethodName} => {Thought}", nameof(ExecuteThoughtAsync), thought))
        {
            // Interpolate arguments based on the context passed.
            foreach (var argument in thought.Arguments)
            {
                thought.Arguments[argument.Key] = argument.Value;

                foreach (var variable in state)
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

            _logger.LogInformation("Invoking {Thought} now.", thought);
            var resultTask = (Task<string>)methodInfo.Invoke(thoughtInstance, parameters.ToArray());

            return resultTask;
        }
    }
}
