using Semantic.Consts.Cognitive;
using Semantic.Extensions.Cognitive;
using Semantic.Interfaces;
using Semantic.Skills.Cognitive;

namespace Semantic.Skills.Core
{
    /// <summary>
    /// Take in any problem or query, $INPUT, and generate a comprehensive step by step plan for how to solve for $INPUT.
    /// </summary>
	public class PlannerSkill : BaseSkill
    {
        /// <summary>
        /// The description of the expected input.
        /// </summary>
        public override string InputDescription => "A goal to accomplish, a problem to solve or a high-level action to perform.";
        /// <summary>
        /// The function of the skill.
        /// </summary>
        public override string Function => $"Take in any problem or query, {PromptVariables.INPUT}, and generate a comprehensive step by step plan for how to solve for {PromptVariables.INPUT}.";
        /// <summary>
        /// The foundational prompt to generate a plan for a query.
        /// </summary>
        private readonly string _prompt = $"""
            Create an XML plan step by step, to satisfy the goal given.
            To create a plan, follow these steps:

            [STEPS]
                - The plan should be as comprehensive as possible. This means break down problems into their smallest solvable parts. Think of it like functional programming.
                - From a <goal> create a <plan> as a series of <functions>.
                - Before using any function in a plan, check that it is present in the most recent [AVAILABLE FUNCTIONS] list. If it is not, do not use it. Do not assume that any function that was previously defined or used in another plan or in [EXAMPLES] is automatically available or compatible with the current plan.
                - Only use functions that are required for the given goal.
                - A function has an 'input' and an 'output'.
                - The 'output' from each function is automatically passed as 'input' to the subsequent <function>.
                - 'input' does not need to be specified if it consumes the 'output' of the previous function.
                - To save an 'output' from a <function>, to pass into a future <function>, use <function.<<FunctionName>> ... setContextVariable: ""<UNIQUE_VARIABLE_KEY>""/>
                - To save an 'output' from a <function>, to return as part of a plan result, use <function.<<FunctionName>> ... appendToResult: ""RESULT__<UNIQUE_RESULT_KEY>""/>
                - You will also suggest any additional functions that could make the task easier in the future and could potentially be reused in other future queries. These suggestions go inside the <suggested-functions> node.
                - When suggesting new functions, those functions should always be small components that can perform core tasks like HTTP calls, file manipulation, AI model inference etc. Never as large as an entire use case. We want these functions to be reusable by other queries.
                - Append an ""END"" XML comment at the end of the plan.
                - Provide your reasoning as a "reasoning" attribute on the plan and function nodes.
                - Provide constructive critisism as a "critisism" attribute on the plan and function nodes.
                - Repeat steps 0 to 11 but do them as if the suggested functions were in fact already available.
                - Finally reflect on your answers, find areas for improvement and repeat steps 0 to 11 with the reflected, updated plan.
            [END STEPS]

            [EXAMPLES]
            [AVAILABLE FUNCTIONS]
                <function.WriterSkill.Summarize function="Summarize a text blob." input="The text blob to summarize."/>
                <function.LanguageHelpers.TranslateTo function="Translate a text blob from English to 'translate_to_language'." input="The text blob to translate." translate_to_language="The language to translate the input text to."/>
                <function.EmailConnector.LookupContactEmail function="Look up a contact's email address." input="The identifier of the contact to look up."/>
                <function.EmailConnector.EmailTo function="Send an email to an email address." input="The text of the email body." recipient="The recipient email address."/>
            [END AVAILABLE FUNCTIONS]

            <goal>Summarize the input, then translate to japanese and email it to Martin. Also SMS it to him.</goal>
            <plan reasoning="..." critisism="...">
              <function.WriterSkill.Summarize reasoning="..." critisism="..." />
              <function.LanguageHelpers.TranslateTo translate_to_language="Japanese" setContextVariable="TRANSLATED_TEXT" reasoning="..." critisism="..." />
              <function.EmailConnector.LookupContactEmail input="Martin" setContextVariable="EMAIL_CONTACT_RESULT" reasoning="..." critisism="..." />
              <function.EmailConnector.EmailTo input="$TRANSLATED_TEXT" recipient="EMAIL_CONTACT_RESULT" reasoning="..." critisism="..." />
            </plan>
            <suggested-functions>
                <function.MobileConnector.LookupContactPhone input="Look up a contact's phone number." recipient="$PHONE_CONTACT_RESULT" how-suggestion="I would log into the Outlook web portal using a technology like Selenium, navigate to the contacts list, search for the contact and grab the info from there programatically."/>
                <function.MobileConnector.SMSTo input="$TRANSLATED_TEXT" recipient="$PHONE_CONTACT_RESULT" how-suggestion="I would integrate with the Twilio API in order to send smses programatically."/>
            </suggested-functions>

            [END EXAMPLES]

            [AVAILABLE FUNCTIONS]
            {PromptVariables.AVAILABLE_FUNCTIONS}
            [END AVAILABLE FUNCTIONS]

            <goal>{PromptVariables.INPUT}</goal>
        """;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="availableSkills">All available skills.</param>
        public PlannerSkill(IEnumerable<BaseSkill> availableSkills)
            : base(availableSkills)
        { }

        /// <summary>
        /// Take in any problem or query, $INPUT, and generate a comprehensive step by step plan for how to solve for $INPUT.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="context">The chain execution context.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The output string.</returns>
        protected override async Task<string> ExecuteAsync(string input, Dictionary<string, object> context, CancellationToken token = default)
        {
            context[PromptVariables.INPUT] = input;
            context[PromptVariables.AVAILABLE_FUNCTIONS] = _availableSkills
                .ToPromptStrings()
                .Aggregate((l, r) => $"{l}{Environment.NewLine}{r}");

            var llmSkill = _availableSkills.First(s => s is LLMSkill);
            var llmPrompt = _prompt.Contextualize(context);
            var llmResponse = await llmSkill.RunAsync(llmPrompt, context, token);

            return llmResponse;
        }
    }
}
