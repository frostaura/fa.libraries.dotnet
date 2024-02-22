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
/// A chain that can take input prompt and generate an image using the StableDiffusion XL model, save it to a .png file and return the path to the .png file.
/// </summary>
public class TextToImageChain : BaseChain
{
    /// <summary>
    /// An example query that this chain example can be used to solve for.
    /// </summary>
    public override string QueryExample => "Generate a picture of a surfer in a hurricane, fighting off sharks that are on fire.";
    /// <summary>
    /// An example query input that this chain example can be used to solve for.
    /// </summary>
    public override string QueryInputExample => "A surfer in a hurricane, fighting off sharks that are on fire, photo-realistic, motion blur.";
    /// The reasoning for the solution of the chain.
    /// </summary>
    public override string Reasoning => "I can use my code interpreter to create a script to use the StableDiffusion XL model to generate an image from a prompt and sae it to a .png file.";
    /// <summary>
    /// A collection of thoughts.
    /// </summary>
    public override List<Thought> ChainOfThoughts => new List<Thought>
    {
        new Thought
        {
            Action = $"{nameof(CodeInterpreterThoughts)}.{nameof(CodeInterpreterThoughts.InvokePythonAsync)}",
            Reasoning = "I will use my code Python code interpreter to construct a script that can use the StableDiffusion XL model to generate an image for the given prompt, and finally return the path of the file.",
            Critisism = "I need to ensure that I use the correct package versions so that the Python environment has the required dependencies installed and ensure that the prompt used for the SDXL model should be optimized.",
            Arguments = new Dictionary<string, string>
            {
                { "pythonVersion", "3.11.3" },
                { "pipDependencies", "diffusers transformers accelerate omegaconf==2.3.0" },
                { "condaDependencies", "ffmpeg" },
                { "code", """
                        def generate(prompt: str) -> str:
                            try:
                                import torch
                                from diffusers import StableDiffusionXLPipeline
                                import uuid

                                output_file_path: str = f'{str(uuid.uuid4())}.png'
                                pipe = StableDiffusionXLPipeline.from_pretrained(
                                    "stabilityai/stable-diffusion-xl-base-1.0", torch_dtype=torch.float16
                                )
                                pipe = pipe.to("mps")

                                image = pipe(prompt).images[0]
                                image.save(output_file_path)

                                return output_file_path
                            except Exception as e:
                                print(e)
                                raise e

                        def main() -> str:
                            return generate('$input')
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
    public TextToImageChain(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, ILogger<TextToImageChain> logger)
        : base(serviceProvider, semanticKernelLanguageModels, logger)
    { }

    /// <summary>
    /// Take input prompt and generate an image using the StableDiffusion XL model, save it to a .png file and return the path to the .png file.
    /// </summary>
    /// <param name="prompt">The prompt to use to generate an image.</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The path to the .png file.</returns>
    [KernelFunction, Description("Take input prompt and generate an image using the StableDiffusion XL model, save it to a .png file and return the path to the .png file.")]
    public Task<string> GenerateImageAndGetFilePathAsync(
        [Description("The prompt to use to generate an image.")] string prompt,
        CancellationToken token = default)
    {
        return ExecuteChainAsync(prompt.ThrowIfNullOrWhitespace(nameof(prompt)), token: token);
    }
}
