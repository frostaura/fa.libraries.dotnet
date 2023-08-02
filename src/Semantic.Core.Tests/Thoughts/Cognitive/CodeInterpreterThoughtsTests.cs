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
            List<string> pipDependencies = new List<string>();
            List<string> condaDependencies = new List<string>();
            string code = "def main()";

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.InvokeAsync(pythonVersion, pipDependencies, condaDependencies, code));

            Assert.Equal(nameof(pythonVersion), actual.ParamName);
        }

        [Fact]
        public async Task InvokeAsync_WithInvalidPipDependencies_ShouldThrow()
        {
            var logger = Substitute.For<ILogger<CodeInterpreterThoughts>>();
            var instance = new CodeInterpreterThoughts(logger);
            string pythonVersion = "3.8";
            List<string> pipDependencies = default;
            List<string> condaDependencies = new List<string>();
            string code = "def main()";

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.InvokeAsync(pythonVersion, pipDependencies, condaDependencies, code));

            Assert.Equal(nameof(pipDependencies), actual.ParamName);
        }

        [Fact]
        public async Task InvokeAsync_WithInvalidCondaDependencies_ShouldThrow()
        {
            var logger = Substitute.For<ILogger<CodeInterpreterThoughts>>();
            var instance = new CodeInterpreterThoughts(logger);
            string pythonVersion = "3.8";
            List<string> pipDependencies = new List<string>();
            List<string> condaDependencies = default;
            string code = "def main()";

            var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.InvokeAsync(pythonVersion, pipDependencies, condaDependencies, code));

            Assert.Equal(nameof(condaDependencies), actual.ParamName);
        }

        [Fact]
        public async Task InvokeAsync_WithInvalidCode_ShouldThrow()
        {
            var logger = Substitute.For<ILogger<CodeInterpreterThoughts>>();
            var instance = new CodeInterpreterThoughts(logger);
            string pythonVersion = "3.8";
            List<string> pipDependencies = new List<string>();
            List<string> condaDependencies = new List<string>();
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
            string pythonVersion = "3.8";
            List<string> pipDependencies = new List<string>();
            List<string> condaDependencies = new List<string>();

            var actual = await instance.InvokeAsync(pythonVersion, pipDependencies, condaDependencies, code);

            Assert.Equal(expected, actual);
        }
    }
}
