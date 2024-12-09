using System.ComponentModel;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Models.Thoughts;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Cognitive;

/// <summary>
/// A chain that can take an input Audio file path and get the transcribe text.
/// </summary>
public class AudioTranscriptionChain : BaseChain
{
    /// <summary>
    /// An example query that this chain example can be used to solve for.
    /// </summary>
    public override string QueryExample => "Transcribe the file '/absolute/path/example.mp3' and return the transcription text.";
    /// <summary>
    /// An example query input that this chain example can be used to solve for.
    /// </summary>
    public override string QueryInputExample => "/absolute/path/example.mp3";
    /// The reasoning for the solution of the chain.
    /// </summary>
    public override string Reasoning => "I can use my code interpreter to create a script to use the OpenAI Whisper model to transcribe an Audio file at a specific path and return the text transcription.";
    /// <summary>
    /// A collection of thoughts.
    /// </summary>
    public override List<Thought> ChainOfThoughts => new List<Thought>
    {
        new Thought
        {
            Action = $"{nameof(CodeInterpreterThoughts)}.{nameof(CodeInterpreterThoughts.InvokePythonAsync)}",
            Reasoning = "I will use my code Python code interpreter to construct a script that can use the OpenAI Whisper model to transcribe the Audio file.",
            Critisism = "I need to ensure that I use the correct package versions so that the Python environment has the required dependencies installed.",
            Arguments = new Dictionary<string, string>
            {
                { "pythonVersion", "3.8" },
                { "pipDependencies", "pip pydub git+https://github.com/openai/whisper.git" },
                { "condaDependencies", "ffmpeg" },
                { "code", """
                        def transcribe(file_path: str) -> str:
                            import whisper

                            model = whisper.load_model('base')

                            print('Transcribing ' + file_path)

                            try:
                                result = model.transcribe(file_path)

                                return result['text']
                            except Exception as e:
                                print(e)
                                raise e

                        def main() -> str:
                            return transcribe('$input')
                        """ }
            },
            OutputKey = "1"
        },
        new LoopThought
        {
            CollectionKey = "audioFiles",
            ItemKey = "audioFile",
            NestedThoughts = new List<Thought>
            {
                new Thought
                {
                    Action = $"{nameof(CodeInterpreterThoughts)}.{nameof(CodeInterpreterThoughts.InvokePythonAsync)}",
                    Reasoning = "I will use my code Python code interpreter to construct a script that can use the OpenAI Whisper model to transcribe the Audio file.",
                    Critisism = "I need to ensure that I use the correct package versions so that the Python environment has the required dependencies installed.",
                    Arguments = new Dictionary<string, string>
                    {
                        { "pythonVersion", "3.8" },
                        { "pipDependencies", "pip pydub git+https://github.com/openai/whisper.git" },
                        { "condaDependencies", "ffmpeg" },
                        { "code", """
                                def transcribe(file_path: str) -> str:
                                    import whisper

                                    model = whisper.load_model('base')

                                    print('Transcribing ' + file_path)

                                    try:
                                        result = model.transcribe(file_path)

                                        return result['text']
                                    except Exception as e:
                                        print(e)
                                        raise e

                                def main() -> str:
                                    return transcribe('$audioFile')
                                """ }
                    },
                    OutputKey = "transcription"
                }
            }
        }
    };

    /// <summary>
    /// Overloaded constructor to provide dependencies.
    /// </summary>
    /// <param name="serviceProvider">The dependency service provider.</param>
    /// <param name="semanticKernelLanguageModels">A component for communicating with language models.</param>
    /// <param name="logger">Instance logger.</param>
    public AudioTranscriptionChain(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, ILogger<AudioTranscriptionChain> logger)
        : base(serviceProvider, semanticKernelLanguageModels, logger)
    { }

    /// <summary>
    /// Take an input Audio file path and get the transcribe text.
    /// </summary>
    /// <param name="audioFilePath">The absolute path to an Audio file to transcrobe. For example a .mp3 or .wav file path.</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The response from the Python executed code.</returns>
    [KernelFunction, Description("Take an input Audio file path and get the transcribe text.")]
    public Task<string> TranscribeAudioFileAsync(
        [Description("The absolute path to an Audio file to transcrobe. For example a .mp3 or .wav file path.")] string audioFilePath,
        CancellationToken token = default)
    {
        using (_logger.BeginScope("{MethodName}", nameof(TranscribeAudioFileAsync)))
        {
            _logger.LogInformation("Starting audio transcription for the file {FileName}", audioFilePath);
            return ExecuteChainAsync(audioFilePath.ThrowIfNullOrWhitespace(nameof(audioFilePath)), token: token);
        }
    }

    /// <summary>
    /// Take a collection of input Audio file paths and get the transcribe text for each.
    /// </summary>
    /// <param name="audioFilePaths">The JSON array of absolute paths to Audio files to transcribe. For example a .mp3 or .wav file paths.</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The JSON array of transcriptions.</returns>
    [KernelFunction, Description("Take a collection of input Audio file paths and get the transcribe text for each.")]
    public Task<string> TranscribeAudioFilesAsync(
        [Description("The JSON array of absolute paths to Audio files to transcribe. For example a .mp3 or .wav file paths.")] string audioFilePaths,
        CancellationToken token = default)
    {
        using (_logger.BeginScope("{MethodName}", nameof(TranscribeAudioFilesAsync)))
        {
            _logger.LogInformation("Starting audio transcription for the files {FileNames}", audioFilePaths);
            return ExecuteChainAsync(audioFilePaths.ThrowIfNullOrWhitespace(nameof(audioFilePaths)), token: token);
        }
    }
}

/// <summary>
/// Mermaid diagram for the AudioTranscriptionChain class.
/// </summary>
/// <remarks>
/// This diagram provides a visual representation of the AudioTranscriptionChain class, its methods, and their interactions.
/// </remarks>
/// <code>
/// classDiagram
///     class AudioTranscriptionChain {
///         +AudioTranscriptionChain(IServiceProvider, ISemanticKernelLanguageModelsDataAccess, ILogger<AudioTranscriptionChain>)
///         +Task~string~ TranscribeAudioFileAsync(string, CancellationToken)
///         +Task~string~ TranscribeAudioFilesAsync(string, CancellationToken)
///     }
///     AudioTranscriptionChain --> BaseChain
///     BaseChain <|-- AudioTranscriptionChain
///     AudioTranscriptionChain : +string QueryExample
///     AudioTranscriptionChain : +string QueryInputExample
///     AudioTranscriptionChain : +string Reasoning
///     AudioTranscriptionChain : +List~Thought~ ChainOfThoughts
/// </code>
