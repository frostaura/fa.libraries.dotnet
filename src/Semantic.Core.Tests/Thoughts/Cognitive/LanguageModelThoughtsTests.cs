using System;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Models.Prompts;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Semantic.Core.Tests.Thoughts.Cognitive;

public class LanguageModelThoughtsTests
{
    [Fact]
    public void Constructor_WithInvalidServiceProvider_ShouldThrow()
    {
        IServiceProvider serviceProvider = null;
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        ILogger<LanguageModelThoughts> logger = Substitute.For<ILogger<LanguageModelThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, logger));

        Assert.Equal(nameof(serviceProvider), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidISemanticKernelLanguageModelsDataAccess_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = null;
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        ILogger<LanguageModelThoughts> logger = Substitute.For<ILogger<LanguageModelThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, logger));

        Assert.Equal(nameof(semanticKernelLanguageModels), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidHttpClientFactory_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = null;
        ILogger<LanguageModelThoughts> logger = Substitute.For<ILogger<LanguageModelThoughts>>();

        var actual = Assert.Throws<ArgumentNullException>(() => new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, logger));

        Assert.Equal(nameof(httpClientFactory), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithInvalidLogger_ShouldThrow()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        ILogger<LanguageModelThoughts> logger = null;

        var actual = Assert.Throws<ArgumentNullException>(() => new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, logger));

        Assert.Equal(nameof(logger), actual.ParamName);
    }

    [Fact]
    public void Constructor_WithValidParams_ShouldConstruct()
    {
        IServiceProvider serviceProvider = Substitute.For<IServiceProvider>();
        ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        IHttpClientFactory httpClientFactory = Substitute.For<IHttpClientFactory>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();

        var actual = new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, logger);

        Assert.NotNull(actual);
    }

    [Fact]
    public async Task PromptLLMAsync_WithInvalidInput_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection().AddSemanticCore(out var configuration);
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = Substitute.For<IHttpClientFactory>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceCollection.BuildServiceProvider(), semanticKernelLanguageModels, httpClientFactory, logger);
        string prompt = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PromptLLMAsync(prompt));

        Assert.Equal(nameof(prompt), actual.ParamName);
    }

    [Fact]
    public async Task PromptLLMAsync_WithValidInput_ShouldRespond()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton(Substitute.For<IUserProxyDataAccess>());
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = serviceProvider.GetRequiredService<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, logger);
        var prompt = "Answer the following as directly as possible: 2+2=";

        var actual = await instance.PromptLLMAsync(prompt);

        Assert.Contains(4.ToString(), actual);
    }

    [Fact]
    public async Task PromptSmallLLMAsync_WithInvalidInput_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton(Substitute.For<IUserProxyDataAccess>());
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = serviceProvider.GetRequiredService<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceCollection.BuildServiceProvider(), semanticKernelLanguageModels, httpClientFactory, logger);
        string prompt = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.PromptSmallLLMAsync(prompt));

        Assert.Equal(nameof(prompt), actual.ParamName);
    }

    [Fact]
    public async Task PromptSmallLLMAsync_WithValidInput_ShouldRespond()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton(Substitute.For<IUserProxyDataAccess>());
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = serviceProvider.GetRequiredService<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceCollection.BuildServiceProvider(), semanticKernelLanguageModels, httpClientFactory, logger);
        var prompt = "Answer the following as directly as possible: 2+2=";

        var actual = await instance.PromptSmallLLMAsync(prompt);

        Assert.Contains(4.ToString(), actual);
    }

    [Fact]
    public async Task GetStringEmbeddingsAsync_WithInvalidInput_ShouldThrow()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton(Substitute.For<IUserProxyDataAccess>());
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, logger);
        string input = default;

        var actual = await Assert.ThrowsAsync<ArgumentNullException>(async () => await instance.GetStringEmbeddingsAsync(input));

        Assert.Equal(nameof(input), actual.ParamName);
    }

    [Fact]
    public async Task GetStringEmbeddingsAsync_WithValidInput_ShouldRespond()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton(Substitute.For<IUserProxyDataAccess>());
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = Substitute.For<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, logger);
        var prompt = "Answer the following as directly as possible: 2+2=";

        var actual = await instance.GetStringEmbeddingsAsync(prompt);

        Assert.NotEmpty(actual);
    }

    [Fact]
    public async Task Function_Call_Test()
    {
        var serviceCollection = new ServiceCollection()
            .AddSemanticCore(out var configuration)
            .AddSingleton(Substitute.For<IUserProxyDataAccess>());
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var semanticKernelLanguageModels = serviceProvider.GetRequiredService<ISemanticKernelLanguageModelsDataAccess>();
        var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        var logger = Substitute.For<ILogger<LanguageModelThoughts>>();
        var instance = new LanguageModelThoughts(serviceProvider, semanticKernelLanguageModels, httpClientFactory, logger);
        //var prompt = "Generate me a picture of an indian young adult version of Tinkerbell from Peter Pan. She should have caramel-shade skin tone. She should be flying in the cosmos with a river close by and lots of reflections. With DallE 3 please.";
        //var prompt = "Give me the C# code for downloading an image from a URL, to my local file system as a PNG file using HttpClient.";
        //var prompt = "What is the current time and date? Give this to me in the format dd->mm->yyyy. (Yes, in that funky -> format. Hint: use code execution)";
        //var prompt = "Synthesize the following for me into an audio file: Hey Emile, nice to meet you. I am Emma, do you have any coding questions for me?";
        //var prompt = "Give me the latest trending tech news from TechCrunch's website and save it as a markdown table in a file named './news.md'.";
        /*var prompt = new CoStarPrompt(
            context: "I want to write a Medium blog post about some code that I wrote in C#. I want it to be detailed and clear. All the code provided should be explained and finally ",
            "STY")*/
        var code = """"
            using System.ComponentModel;
            using FrostAura.Libraries.Core.Extensions.Validation;
            using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
            using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
            using FrostAura.Libraries.Semantic.Core.Models.Thoughts;
            using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
            using FrostAura.Libraries.Semantic.Core.Thoughts.IO;
            using Microsoft.Extensions.Logging;
            using Microsoft.SemanticKernel;

            namespace FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Cognitive;

            /// <summary>
            /// A chain that can take input text, synthesize speech for the text, save it to a .wav file and return the path to the .wav file.
            /// </summary>
            public class TextToSpeechChain : BaseChain
            {
                /// <summary>
                /// An example query that this chain example can be used to solve for.
                /// </summary>
                public override string QueryExample => "Synthesize 'This is a hello world example' to speech.";
                /// <summary>
                /// An example query input that this chain example can be used to solve for.
                /// </summary>
                public override string QueryInputExample => "This is a hello world example";
                /// The reasoning for the solution of the chain.
                /// </summary>
                public override string Reasoning => "I can use my code interpreter to create a script to use the XTTS model (Using the TTS library) in order to generate a .wav file with the synthesized speech for the given text.";
                /// <summary>
                /// A collection of thoughts.
                /// </summary>
                public override List<Thought> ChainOfThoughts => new List<Thought>
                {
                    new Thought
                    {
                        Action = $"{nameof(CodeInterpreterThoughts)}.{nameof(CodeInterpreterThoughts.InvokeAsync)}",
                        Reasoning = "I will use my code Python code interpreter to construct a script that can use the XTTS model via the TTS library and synthesize speech, and finally return the path of the file.",
                        Critisism = "I need to ensure that I use the correct package versions so that the Python environment has the required dependencies installed and ensure that I have a voice to reference to speak with.",
                        Arguments = new Dictionary<string, string>
                        {
                            { "pythonVersion", "3.10" },
                            { "pipDependencies", "TTS" },
                            { "condaDependencies", "ffmpeg" },
                            { "code", """
                                    def download_and_get_speaker_voice_wav_file_path() -> str:
                                        import requests

                                        # We can use one of the open source voices off the Tortoise TTS project.
                                        voice_file_download_url: str = 'https://github.com/neonbjb/tortoise-tts/raw/main/tortoise/voices/emma/1.wav'

                                        # Download the voice file and save it locally.
                                        response = requests.get(voice_file_download_url)
                                        output_file_name: str = 'voice_to_speak.wav'

                                        print(f'Downloading voice "{voice_file_download_url}" to "{output_file_name}".')

                                        with open(output_file_name, 'wb') as f:
                                            f.write(response.content)

                                        return output_file_name

                                    def synthesize(text: str) -> str:
                                        try:
                                            from TTS.api import TTS
                                            import uuid

                                            voice_file_path: str = download_and_get_speaker_voice_wav_file_path()

                                            print(f'Synthesizing text "{text}" with voice "{voice_file_path}".')

                                            tts: TTS = TTS('tts_models/multilingual/multi-dataset/xtts_v2')
                                            output_file_path: str = f'{str(uuid.uuid4())}.wav'
                                            result: str = tts.tts_to_file(
                                                text=text,
                                                file_path=output_file_path,
                                                speaker_wav=voice_file_path,
                                                language="en",
                                                split_sentences=False
                                            )

                                            return result
                                        except Exception as e:
                                            print(e)
                                            raise e

                                    def main() -> str:
                                        import os

                                        os.environ['REQUESTS_CA_BUNDLE'] = '/Library/Application Support/Netskope/STAgent/download/nscacert_combined.pem'
                                        os.environ['SSL_CERT_FILE'] = '/Library/Application Support/Netskope/STAgent/download/nscacert_combined.pem'

                                        return synthesize('$input')
                                    """ }
                        },
                        OutputKey = "1"
                    },
                    new Thought
                    {
                        Action = $"{nameof(SystemThoughts)}.{nameof(SystemThoughts.OutputTextAsync)}",
                        Reasoning = "I can simply proxy the response as a direct and response is appropriate for an exact transcription.",
                        Arguments = new Dictionary<string, string>
                        {
                            { "output", "$1" }
                        },
                        OutputKey = "2"
                    }
                };

                /// <summary>
                /// Overloaded constructor to provide dependencies.
                /// </summary>
                /// <param name="serviceProvider">The dependency service provider.</param>
                /// <param name="semanticKernelLanguageModels">A component for communicating with language models.</param>
                /// <param name="logger">Instance logger.</param>
                public TextToSpeechChain(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, ILogger<TextToSpeechChain> logger)
                    : base(serviceProvider, semanticKernelLanguageModels, logger)
                { }

                /// <summary>
                /// Take input text, synthesize speech for the text, save it to a .wav file and return the path to the .wav file.
                /// </summary>
                /// <param name="text">The text to synthesize speech for.</param>
                /// <param name="token">The token to use to request cancellation.</param>
                /// <returns>The path to the .wav file.</returns>
                [KernelFunction, Description("Take input text, synthesize speech for the text, save it to a .wav file and return the path to the .wav file.")]
                public Task<string> SpeakTextAndGetFilePathAsync(
                    [Description("The text to synthesize speech for.")] string text,
                    CancellationToken token = default)
                {
                    return ExecuteChainAsync(text.ThrowIfNullOrWhitespace(nameof(text)), token: token);
                }
            }
            """";
        var blogName = "Text to Speech from C# using and XTTS v2 (Python), with Chains & CodeInterpreterThoughts";
        var prompt = $"""
            You are a professional Medium blog writer. Your job is to take a given code blob and create a blog for the Medium platform.

            Steps:
            * Generate a blog for the Medium platform for the Problem Statement as specified below.
            * Your output should be in Markdown format.
            * Ensure to elaborate on the problem statement and why the content of the blog is relevant or important.
            * Your blog should be factual.
            * Include all the code as you explain it and bring it all together at the end, this is a how-to-code-the-solution blog.
            * Your blog may include but not limited to describing the code in further detail so the median software engineer may understand.
            * Keep the blog content as concise as possible without removing any important information.
            * Feel free to include your own comments/descriptions etc of the code as specified below.
            * Your content should specify the title and tags for the blog too.
            * If there are nested bode blocks of another language (for example python code in a string), that code should also be explained.

            Problem Statement: The below is code that allows for {blogName}.

            Code (C#):
            {code}
            """;
        prompt = "Give me a concise summary of https://www.msn.com/en-us/news/politics/ron-desantis-launched-his-white-house-run-by-bashing-disney-but-his-foolish-fatal-error-has-given-ceo-bob-iger-the-last-laugh/ar-BB1h4w9X";

        var actual = await instance.PromptSmallLLMAsync(prompt);

        Assert.NotEmpty(actual);
    }
}
