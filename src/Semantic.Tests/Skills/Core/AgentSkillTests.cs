using System;
using Semantic.Skills.Cognitive;
using Semantic.Skills.Core;
using Xunit;

namespace Semantic.Tests.Skills.Core
{
	public class TaskPlannerSkillTests
	{
        [Fact]
        public void Construct_WithNoParams_ShouldConstruct()
        {
            var llmSkill = new AzureOpenAILargeLanguageModelSkill();
            var inst = new TaskPlannerSkill(llmSkill);

            Assert.NotNull(inst);
        }

        [Theory]
        [InlineData("Write me a book about a currently trending topic, including ai-generating the cover art and finally self-publishing it to Amazon.")]
        [InlineData("Create me an end-to-end YouTube short video on a currently trending topic, ai-generating any video and audio requirements and finally auto-uploading it to my YouTube channel.")]
        public async Task RunAsync_WithValidInput_ShouldRespond(string input)
        {
            var context = new Dictionary<string, object>();
            var llmSkill = new AzureOpenAILargeLanguageModelSkill();
            var inst = new TaskPlannerSkill(llmSkill);
            var response = await inst.RunAsync(input, context);

            Assert.NotEmpty(response);
        }
    }
}

