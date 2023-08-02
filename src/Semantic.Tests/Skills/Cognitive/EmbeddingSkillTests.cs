using Semantic.Extensions.Cognitive;
using Semantic.Skills;
using Semantic.Skills.Cognitive;
using Xunit;

namespace Semantic.Tests.Skills.Cognitive;

public class LLMSkillTests
{
    [Fact]
    public void Construct_WithNoParams_ShouldConstruct()
    {
        var skills = typeof(BaseSkill)
            .Assembly
            .GetAllSkillTypesInAssembly()
            .CreateInstances()
            .ToList();
        var inst = new LLMSkill(skills);

        Assert.NotNull(inst);
    }

    [Fact]
    public async Task RunAsync_WithValidInput_ShouldRespond()
    {
        var skills = typeof(BaseSkill)
            .Assembly
            .GetAllSkillTypesInAssembly()
            .CreateInstances()
            .ToList();
        var context = new Dictionary<string, object>();
        var input = "Hello, my name is Dean";
        var inst = new LLMSkill(skills);
        var response = await inst.RunAsync(input, context);

        Assert.NotEmpty(response);
    }
}
