using Microsoft.Extensions.Logging;
using Semantic.Extensions.Cognitive;
using Semantic.Extensions.Logging;
using Semantic.Skills;
using Semantic.Skills.Core;
using Xunit;

namespace Semantic.Tests.Skills.Data;

public class UrlScraperSkillTests
{
    [Fact]
    public void Construct_WithNoParams_ShouldConstruct()
    {
        var logger = (ILogger<HttpGetSkill>)ILoggerExtensions.CreateDefaultLogger(typeof(HttpGetSkill));
        var skills = typeof(BaseSkill)
            .Assembly
            .GetAllSkillTypesInAssembly()
            .CreateInstances()
            .ToList();
        var inst = new HttpGetSkill(skills, logger);

        Assert.NotNull(inst);
    }

    [Fact]
    public async Task RunAsync_WithValidInput_ShouldRespond()
    {
        var logger = (ILogger<HttpGetSkill>)ILoggerExtensions.CreateDefaultLogger(typeof(HttpGetSkill));
        var skills = typeof(BaseSkill)
            .Assembly
            .GetAllSkillTypesInAssembly()
            .CreateInstances()
            .ToList();
        var context = new Dictionary<string, string>();
        var input = "https://www.google.com.au/search?q=Latest%20trending%20book%20topics";
        var inst = new HttpGetSkill(skills, logger);
        var response = await inst.RunAsync(input, context);

        Assert.NotEmpty(response);
    }
}
