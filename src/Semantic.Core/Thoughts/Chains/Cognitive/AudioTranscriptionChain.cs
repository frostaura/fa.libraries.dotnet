using System.ComponentModel;
using FrostAura.Libraries.Semantic.Core.Models.Thoughts;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using FrostAura.Libraries.Semantic.Core.Thoughts.IO;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.SkillDefinition;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Chains.Cognitive
{
	public class AudioTranscriptionChain : BaseExecutableChain
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
                Action = $"{nameof(CodeInterpreterThoughts)}.{nameof(CodeInterpreterThoughts.InvokeAsync)}",
                Reasoning = "I will use my code Python code interpreter to construct a script that can use the OpenAI Whisper model to transcribe the Audio file.",
                Critisism = "I need to ensure that I use the correct package versions so that the Python environment has the required dependencies installed.",
                Arguments = new Dictionary<string, string>
                {
                    { "pythonVersion", "3.8" },
                    { "pipDependencies", "pydub git+https://github.com/openai/whisper.git" },
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
            new Thought
            {
                Action = $"{nameof(OutputThoughts)}.{nameof(OutputThoughts.OutputTextAsync)}",
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
        /// <param name="logger">Instance logger.</param>
        public AudioTranscriptionChain(IServiceProvider serviceProvider, ILogger<AudioTranscriptionChain> logger)
            : base(serviceProvider, logger)
        { }

        /// <summary>
        /// Take an input Audio file path and get the transcribe text.
        /// </summary>
        /// <param name="audioFilePath">The absolute path to an Audio file to transcrobe. For example a .mp3 or .wav file path.</param>
        /// <param name="token">The token to use to request cancellation.</param>
        /// <returns>The response from the Python executed code.</returns>
        [SKFunction, Description("Take an input Audio file path and get the transcribe text.")]
        public Task<string> TranscribeAudioFileAsync(
            [Description("The absolute path to an Audio file to transcrobe. For example a .mp3 or .wav file path.")] string audioFilePath,
            CancellationToken token = default)
        {
            return ExecuteChainAsync(audioFilePath, token: token);
        }
    }
}
