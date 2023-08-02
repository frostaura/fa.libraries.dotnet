using FrostAura.Libraries.Core.Extensions.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.AI.ChatCompletion;
using Newtonsoft.Json;
using Semantic.Core.Attribute.Functions;
using Semantic.Core.Constants.Functions;
using Semantic.Core.Enumerations.Semantic;
using Semantic.Core.Extensions.Functions;
using Semantic.Core.Functions.Semantic.Memory;
using Semantic.Core.Models.Semantic;

namespace Semantic.Core.Functions.Semantic.Agents
{
    /// <summary>
    /// Allows for passing an input, a general question or request to action and solve for it step-by-step.
    /// </summary>
    public class ReActAgent : FunctionCore
	{
        /// <summary>
        /// The purpose of the function or the kind of solution it provides.
        /// </summary>
        public override string Purpose => $"Allows for passing an input, a general question or request to action and solve for it step-by-step, one step at a time.";
        /// <summary>
        /// The semantic kernel to use for LLM calls.
        /// </summary>
        private readonly IKernel _kernel;
        /// <summary>
        /// The dependency service provider.
        /// </summary>
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// The prompt template to use for the LLM call.
        /// </summary>
        private string _prompt = @"
            Guidance:
            - Generate only the next thought and action.
            - Actions support zero or more string arguments, escaped by ' and delimited by a comma.
            - Only use actions available to you as stated below.

            Available Actions (In No Order):
            Lookup[term='The terms to look up.'] // Used elaborate on something even further. To look up additional information.
            {AVAILABLE_ACTIONS}
 
            Question: What is the elevation range for the area that the eastern sector of the Colorado orogeny extends into?

            Thought: I need to search Colorado orogeny, find the area that the eastern sector of the Colorado orogeny extends into, then find the elevation range of the area.
            Action: Wikipedia[arg_input='Colorado orogeny']
            Observation 1:
            ---
            The Colorado orogeny was an episode of mountain building (an orogeny) in Colorado and surrounding areas.
            ---

            Thought: It does not mention the eastern sector. So I need to look up eastern sector.
            Action: Lookup[term='eastern sector']
            Observation:
            ---
            (Result 1 / 1) The eastern sector extends into the High Plains and is called the Central Plains orogeny.
            ---

            Thought: The eastern sector of Colorado orogeny extends into the High Plains. So I need to search High Plains and find its elevation range.
            Action: Wikipedia[arg_input='High Plains']
            Observation:
            ---
            High Plains refers to one of two distinct land regions.
            ---

            Thought: I need to instead search High Plains (United States).
            Action: Wikipedia[arg_input='High Plains (United States)']
            Observation:
            ---
            The High Plains are a subregion of the Great Plains. From east to west, the High Plains rise in elevation from around 1,800 to 7,000 ft (550 to 2,130m).
            ---

            Thought: High Plains rise in elevation from around 1,800 to 7,000 ft, so the answer is 1,800 to 7,000 ft.
            Action: OutputText[arg_result='1,800 to 7,000 ft']
            Observation:
            ---
            Finished.
            ---

            Question: {QUESTION}
            
        ";

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="serviceProvider">The dependency service provider.</param>
        /// <param name="kernel">The semantic kernel to use for LLM calls.</param>
        /// <param name="logger">Instance logger.</param>
        public ReActAgent(IServiceProvider serviceProvider, IKernel kernel, ILogger<ReActAgent> logger)
            : base(logger)
        {
            _kernel = kernel
                .ThrowIfNull(nameof(kernel));
            _serviceProvider = serviceProvider
                .ThrowIfNull(nameof(serviceProvider));
        }

        /// <summary>
        /// Allows for passing an input, a general question or request to action and solve for it step-by-step.
        /// </summary>
        /// <param name="arguments">The required arguments provided.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The output string.</returns>
        [Argument(ArgumentNames.INPUT, "A general question or request to action and solve for it step-by-step.")]
        protected override async Task<string> ExecuteAsync(Dictionary<string, string> arguments, CancellationToken token = default)
        {
            var input = arguments.GetArgument(ArgumentNames.INPUT);
            var model = _kernel.GetService<IChatCompletion>(ModelType.LargeLLM.ToString());
            var initialPrompt = _prompt.Replace("{QUESTION}", input);
            var thoughts = new List<GeneratedThought>();

            do
            {
                // Append current problem solving history to the base template.
                var currentPrompt = $"{initialPrompt}";

                if (thoughts.Any())
                {
                    currentPrompt = initialPrompt += thoughts
                        .Select(t => t.ToString())
                        .Aggregate((l, r) => l + Environment.NewLine + r);
                }

                // Produce the next thought.
                var response = await GenerateNextThoughtAsync(initialPrompt, model, token);
                var thought = new GeneratedThought
                {
                    Thought = response
                        .Split(Environment.NewLine)
                        .First()
                        .Split(":")
                        .Last()
                        .TrimStart(),
                    Action = response
                        .Split(Environment.NewLine)
                        .Last()
                        .Split(":")
                        .Last()
                        .TrimStart(),
                    Observation = default
                };

                // Perform action and save the observation.
                var action = GetActionFromName(thought.Action.Split("[").First());

                if (action == default) throw new ArgumentNullException($"No action could be resolved for '{thought.Action}'.");

                var args = thought
                    .Action
                    .Split("[")
                    .Last()
                    .Split("]")
                    .First()
                    .Split(",")
                    .Distinct()
                    .ToDictionary(a => a.Split("='").First().Trim(), a => a.Split("='").Last().Split("'").First());
                var observation = await action.RunAsync(args, token);
                thought.Observation = observation;

                thoughts.Add(thought);
            }
            while (!thoughts.Last().IsFinished);

            return thoughts.Last().Observation;
        }

        /// <summary>
        /// Step in the ReAct pipeline to generate a thought.
        /// </summary>
        /// <param name="context">The absolute prompt to execute and the context of the chain of thought thus far.</param>
        /// <param name="model">Chat model to utilize.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The generated thought response.</returns>
        private async Task<string> GenerateNextThoughtAsync(string context, IChatCompletion model, CancellationToken token)
        {
            var chat = model.CreateNewChat();
            var formattedPrompt = _prompt
                .Replace("{AVAILABLE_ACTIONS}", GetAvailableActionsString());

            chat.AddUserMessage(context);

            var response = await model.GenerateMessageAsync(chat, cancellationToken: token);

            return response;
        }

        /// <summary>
        /// Get an action from it's name.
        /// </summary>
        /// <param name="name">Name of the action.</param>
        /// <returns>The action instance.</returns>
        private FunctionCore GetActionFromName(string name)
        {
            var action = (_serviceProvider
                .GetService(typeof(IEnumerable<FunctionCore>)) as IEnumerable<FunctionCore>)
                .FirstOrDefault(a => a.GetType().Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            return action;
        }

        /// <summary>
        /// Get a formatted string representation of all available actions.
        /// </summary>
        /// <returns></returns>
        private string GetAvailableActionsString()
        {
            var availableActions = (_serviceProvider
                .GetService(typeof(IEnumerable<FunctionCore>)) as IEnumerable<FunctionCore>);
            var availableFunctionsString = availableActions
                .Select(a => a.ToString())
                .Aggregate((l, r) => l + Environment.NewLine + r);

            return availableFunctionsString;
        }
    }
}
