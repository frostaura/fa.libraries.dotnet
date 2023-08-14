using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Semantic.Core.Tests.Thoughts.Cognitive
{
    public class CodeInterpreterThoughtsTests
    {
		[Fact]
        public void Constructor_WithInvalidLogger_ShouldThrow()
        {
            ILogger<CodeInterpreterThoughts> logger = null;

            var actual = Assert.Throws<ArgumentNullException>(() => new CodeInterpreterThoughts(logger));

            Assert.Equal(nameof(logger), actual.ParamName);
        }

        [Fact]
        public void Constructor_WithValidParams_ShouldConstruct()
        {
            var logger = Substitute.For<ILogger<CodeInterpreterThoughts>>();

            var actual = new CodeInterpreterThoughts(logger);

            Assert.NotNull(actual);
        }

        [Fact]
        public async Task InvokeAsync_WithInvalidPythonVersion_ShouldThrow()
        {
            var logger = Substitute.For<ILogger<CodeInterpreterThoughts>>();
            var instance = new CodeInterpreterThoughts(logger);
            string pythonVersion = default;
            string pipDependencies = "";
            string condaDependencies = "";
            string code = "def main()";

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.InvokeAsync(pythonVersion, pipDependencies, condaDependencies, code));

            Assert.Equal(nameof(pythonVersion), actual.ParamName);
        }

        [Fact]
        public async Task InvokeAsync_WithInvalidCode_ShouldThrow()
        {
            var logger = Substitute.For<ILogger<CodeInterpreterThoughts>>();
            var instance = new CodeInterpreterThoughts(logger);
            string pythonVersion = "3.8";
            string pipDependencies = "";
            string condaDependencies = "";
            string code = default;

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.InvokeAsync(pythonVersion, pipDependencies, condaDependencies, code));

            Assert.Equal(nameof(code), actual.ParamName);
        }

        [Theory]
        [InlineData("""
            def main() -> str:
                return 'Test 1'
            """, "Test 1")]
        [InlineData("""
            def main() -> str:
                return f'{1 + 1}'
            """, "2")]
        [InlineData("""
            def main() -> str:
                return f'{True}'
            """, "True")]
        public async Task InvokeAsync_WithValidParams_ShouldExecuteAndRespondCorrectly(string code, string expected)
        {
            var logger = Substitute.For<ILogger<CodeInterpreterThoughts>>();
            var instance = new CodeInterpreterThoughts(logger);
            string pythonVersion = "3.9";
            string pipDependencies = "pip";
            string condaDependencies = "ffmpeg";

            var actual = await instance.InvokeAsync(pythonVersion, pipDependencies, condaDependencies, code);

            Assert.Equal(expected, actual);
        }
    }
}
