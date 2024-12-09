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
            Action = $"{nameof(CodeInterpreterThoughts)}.{nameof(CodeInterpreterThoughts.InvokePythonAsync)}",
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
                            return synthesize('$input')
                        """ }
            },
            OutputKey = "1"
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
        using (_logger.BeginScope("{MethodName}", nameof(SpeakTextAndGetFilePathAsync)))
        {
            _logger.LogInformation("Starting speech synthesis for {Text}", text);
            return ExecuteChainAsync(text.ThrowIfNullOrWhitespace(nameof(text)), token: token);
        }
    }
}

/// <summary>
/// Mermaid diagram for the TextToSpeechChain class.
/// </summary>
/// <remarks>
/// This diagram provides a visual representation of the TextToSpeechChain class, its methods, and their interactions.
/// </remarks>
/// <code>
/// classDiagram
///     class TextToSpeechChain {
///         +TextToSpeechChain(IServiceProvider, ISemanticKernelLanguageModelsDataAccess, ILogger<TextToSpeechChain>)
///         +Task~string~ SpeakTextAndGetFilePathAsync(string, CancellationToken)
///     }
///     TextToSpeechChain --> BaseChain
///     BaseChain <|-- TextToSpeechChain
///     TextToSpeechChain : +string QueryExample
///     TextToSpeechChain : +string QueryInputExample
///     TextToSpeechChain : +string Reasoning
///     TextToSpeechChain : +List~Thought~ ChainOfThoughts
/// </code>
