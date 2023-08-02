using Microsoft.Extensions.Logging;
using Semantic.Consts.Cognitive;
using Semantic.Extensions.Cognitive;
using Semantic.Interfaces;
using Semantic.Skills.Cognitive;

namespace Semantic.Skills.Core
{
    /// <summary>
    /// Take in any problem or query, $INPUT, and generate a comprehensive step by step plan for how to solve for $INPUT.
    /// </summary>
	public class PlannerV2Skill : BaseSkill
    {
        /// <summary>
        /// The description of the expected input.
        /// </summary>
        public override string InputDescription => "A goal to accomplish, a problem to solve or a high-level action to perform.";
        /// <summary>
        /// The function of the skill.
        /// </summary>
        public override string Function => $"Take in any problem or query, {PromptVariables.INPUT}, and generate a comprehensive step by step plan for how to solve for {PromptVariables.INPUT}. This skill is great when it seems like no other skills could solve a problem because this skill can break down a problem into smaller skill requirements automatically managing the execution of those skills to return with a coherent response.";
        /// <summary>
        /// The steps that the planner should follow to produce the plan.
        /// </summary>
        private List<string> _plannerSteps = new List<string>
        {
            "The plan should be as comprehensive as possible. This means break down problems into their smallest solvable parts. Think of it like functional programming.",
            "From a <goal> create a <plan> as a series of <skill>.",
            "Before using any skill in a plan, check that it is present in the most recent [AVAILABLE SKILLS] list. If it is not, do not use it. Do not assume that any skill that was previously defined or used in another plan or in [EXAMPLES] is automatically available or compatible with the current plan.",
            "Only use skills that are required for the given goal.",
            "A skill has an 'input' and an 'output'.",
            "The 'output' from each skill is automatically passed as 'input' to the subsequent <skill>.",
            "'input' does not need to be specified if it consumes the 'output' of the previous skill.",
            "To save an 'output' from a <skill>, to pass into a future <skill>, use <skill.<<SkillName>> ... setContextVariable: \"\"<UNIQUE_VARIABLE_KEY>\"\"/>",
            "You will also suggest any additional skills that could make the task easier in the future and could potentially be reused in other future queries. These suggestions go inside the <suggested-skills> node.",
            "When suggesting new skills, those skills should always be small components that can perform core tasks like HTTP calls, file manipulation, AI model inference etc. Never as large as an entire use case. We want these skills to be reusable by other queries.",
            "Append an \"\"END\"\" XML comment at the end of the plan.",
            "Provide your reasoning as a \"reasoning\" attribute on the <plan> and <skill>.",
            "Provide constructive critisism as a \"critisism\" attribute on <plan> and <skill>.",
            "Repeat steps 0 to 11 but do them as if the suggested skills were in fact already available.",
            "Finally reflect on your answers, find areas for improvement and repeat steps 0 to 11 with the reflected, updated plan. Enclose the final <plan> in a <final> node."
        };
        /// <summary>
        /// The foundational prompt to generate a plan for a query.
        /// </summary>
        private string _prompt => $"""
            Create an XML plan step by step, to satisfy the goal given.
            To create a plan, follow these steps:

            [STEPS]
                {_plannerSteps.Select(ps => $"- {ps}").Aggregate((l, r) => $"{l}{Environment.NewLine}{r}")}
            [END STEPS]

            [EXAMPLES]
                [EXAMPLE 1]
                    [AVAILABLE SKILLS]
                        <skill.WriterSkill.Summarize function="Summarize a text blob." input="The text blob to summarize."/>
                        <skill.LanguageHelpers.TranslateTo function="Translate a text blob from English to 'translate_to_language'." input="The text blob to translate." translate_to_language="The language to translate the input text to."/>
                        <skill.EmailConnector.LookupContactEmail function="Look up a contact's email address." input="The identifier of the contact to look up."/>
                        <skill.EmailConnector.EmailTo function="Send an email to an email address." input="The text of the email body." recipient="The recipient email address."/>
                    [END AVAILABLE SKILLS]

                    [GOAL]Summarize the input, then translate to japanese and email it to Martin. Also SMS it to him.[END GOAL]
                    <plan reasoning="..." critisism="...">
                      <skill.WriterSkill.Summarize reasoning="..." critisism="..." />
                      <skill.LanguageHelpers.TranslateTo translate_to_language="Japanese" setContextVariable="TRANSLATED_TEXT" reasoning="..." critisism="..." />
                      <skill.EmailConnector.LookupContactEmail input="Martin" setContextVariable="EMAIL_CONTACT_RESULT" reasoning="..." critisism="..." />
                      <skill.EmailConnector.EmailTo input="$TRANSLATED_TEXT" recipient="EMAIL_CONTACT_RESULT" reasoning="..." critisism="..." />
                    </plan>
                    <suggested-skills>
                        <skill.MobileConnector.LookupContactPhone input="Look up a contact's phone number." recipient="$PHONE_CONTACT_RESULT" how-suggestion="I would log into the Outlook web portal using a technology like Selenium, navigate to the contacts list, search for the contact and grab the info from there programatically."/>
                        <skill.MobileConnector.SMSTo input="$TRANSLATED_TEXT" recipient="$PHONE_CONTACT_RESULT" how-suggestion="I would integrate with the Twilio API in order to send smses programatically."/>
                    </suggested-skills>
                [END EXAMPLE 1]
            [END EXAMPLES]

            [AVAILABLE SKILLS]
                {PromptVariables.AVAILABLE_SKILLS}
            [END AVAILABLE SKILLS]

            <goal>{PromptVariables.INPUT}</goal>
        """;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="availableSkills">All available skills.</param>
        public PlannerV2Skill(IEnumerable<BaseSkill> availableSkills, ILogger logger)
            : base(availableSkills, logger)
        { }

        /// <summary>
        /// Take in any problem or query, $INPUT, and generate a comprehensive step by step plan for how to solve for $INPUT.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="context">The chain execution context.</param>
        /// <param name="token">A token to allow for cancelling downstream operations.</param>
        /// <returns>The output string.</returns>
        protected override async Task<string> ExecuteAsync(string input, Dictionary<string, string> context, CancellationToken token = default)
        {
            context[PromptVariables.INPUT] = input;
            context[PromptVariables.AVAILABLE_SKILLS] = _availableSkills
                .ToPromptStrings()
                .Aggregate((l, r) => $"{l}{Environment.NewLine}{r}");

            var llmSkill = _availableSkills.First(s => s is LLMSkill);
            var llmPrompt = _prompt.Contextualize(context);
            var llmResponse = await llmSkill.RunAsync(llmPrompt, context, token);

            return llmResponse;
        }
    }
}
