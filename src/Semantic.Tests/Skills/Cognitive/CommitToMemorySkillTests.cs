using Microsoft.Extensions.Logging;
using Semantic.Extensions.Cognitive;
using Semantic.Extensions.Logging;
using Semantic.Skills;
using Semantic.Skills.Cognitive;
using Xunit;

namespace Semantic.Tests.Skills.Cognitive;

public class LLMSkillTests
{
    [Fact]
    public void Construct_WithNoParams_ShouldConstruct()
    {
        var logger = ILoggerExtensions.CreateDefaultLogger(typeof(LLMSkill));
        var skills = typeof(BaseSkill)
            .Assembly
            .GetAllSkillTypesInAssembly()
            .CreateInstances()
            .ToList();
        var inst = new LLMSkill(skills, logger);

        Assert.NotNull(inst);
    }

    [Fact]
    public async Task RunAsync_WithValidInput_ShouldRespond()
    {
        var logger = ILoggerExtensions.CreateDefaultLogger(typeof(LLMSkill));
        var skills = typeof(BaseSkill)
            .Assembly
            .GetAllSkillTypesInAssembly()
            .CreateInstances()
            .ToList();
        var context = new Dictionary<string, string>();
        var input = "Hello, my name is Dean";
        var inst = new LLMSkill(skills, logger);
        var response = await inst.RunAsync(input, context);

        Assert.NotEmpty(response);
    }
}
