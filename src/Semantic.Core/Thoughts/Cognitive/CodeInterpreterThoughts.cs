using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;

/// <summary>
/// Code interpreter thoughts.
/// </summary>
public class CodeInterpreterThoughts : BaseThought
{
    /// <summary>
    /// Overloaded constructor to provide dependencies.
    /// </summary>
    /// <param name="serviceProvider">The dependency service provider.</param>
    /// <param name="semanticKernelLanguageModels">A component for communicating with language models.</param>
    /// <param name="logger">Instance logger.</param>
    public CodeInterpreterThoughts(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, ILogger<CodeInterpreterThoughts> logger)
        :base(serviceProvider, semanticKernelLanguageModels, logger)
    { }

    /// <summary>
    /// Execute Python code against a specific version of Python as well as with specific PIP package requirements.
    /// </summary>
    /// <param name="pythonVersion">The version of Python to execute code against. For example '3.8'.</param>
    /// <param name="pipDependencies">A collection of PIP dependencies the Python code requires. The items can either be a package name (Example: 'pandas==1.2.3') or a package name with its version (Example: 'pandas').</param>
    /// <param name="condaDependencies">A collection of Conda dependencies the Python code requires. The items can either be a package name (Example: 'ffmpeg==1.2.3') or a package name with its version (Example: 'ffmpeg').</param>
    /// <param name="code">Python code to execute. The only requirement for the structure of this code is that the code should be wrapped in a function called 'main' that accepts zero arguments and returns a string.</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The response from the Python executed code.</returns>
    [KernelFunction, Description("Execute Python code against a specific version of Python as well as with specific PIP package requirements.")]
    public async Task<string> InvokeAsync(
        [Description("The version of Python to execute code against. For example '3.8'.")] string pythonVersion,
        [Description("A collection of PIP dependencies the Python code requires. The items can either be a package name (Example: 'pandas==1.2.3') or a package name with its version (Example: 'pandas').")] string pipDependencies,
        [Description("A collection of Conda dependencies the Python code requires. The items can either be a package name (Example: 'ffmpeg==1.2.3') or a package name with its version (Example: 'ffmpeg').")] string condaDependencies,
        [Description("Python code to execute. The only requirement for the structure of this code is that the code MUST be wrapped in a function called 'main' that accepts zero arguments and returns a string. Example: main() -> str ...")] string code,
        CancellationToken token = default)
    {
        if (!code
                .ThrowIfNullOrWhitespace(nameof(code))
                .Contains("def main()")) throw new ArgumentException("The code block is required to contain a function called 'main' (def main() -> str:) that accepts zero arguments and returns a string.", nameof(code));

        var pythonExecutablePath = await EnsurePythonEnvironmentAndGetPathAsync(
            pythonVersion.ThrowIfNullOrWhitespace(nameof(pythonVersion)),
            pipDependencies.Split(' ').ToList(),
            condaDependencies.Split(' ').ToList(),
            token);
        var outputIndicator = "OUTPUT";
        var executableCode = $"""
                {code}

                print('{outputIndicator}>>>' + main() + '<<<{outputIndicator}')
                """;

        Console.WriteLine("Executing Python code against the Conda environment with the required depdencies installed.");

        var response = await ExecutePythonProcessAsync(pythonExecutablePath, executableCode, token);
        var normalizedResponse = response
            .Split($"{outputIndicator}>>>")
            .Last()
            .Split($"<<<{outputIndicator}")
            .First();

        return normalizedResponse;
    }

    /// <summary>
    /// Create a Python environment given a specific Python version and dependencies, if not exists and return the full Python process path.
    /// </summary>
    /// <param name="pythonVersion">The version of Python to execute code against. For example '3.8'.</param>
    /// <param name="pipDependencies">A collection of PIP dependencies the Python code requires. The items can either be a package name (Example: 'pandas==1.2.3') or a package name with its version (Example: 'pandas').</param>
    /// <param name="condaDependencies">A collection of Conda dependencies the Python code requires. The items can either be a package name (Example: 'ffmpeg==1.2.3') or a package name with its version (Example: 'ffmpeg').</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The full Python process path.</returns>
    private async Task<string> EnsurePythonEnvironmentAndGetPathAsync(string pythonVersion, List<string> pipDependencies, List<string> condaDependencies, CancellationToken token)
    {
        var environmentName = GetPythonEnvironmentHash(pythonVersion, pipDependencies, condaDependencies);
        var environmentPath = $"envs/{environmentName}";
        var pythonExecutablePath = $"{environmentPath}/bin/python";

        if (!Directory.Exists(environmentPath))
        {
            Console.WriteLine($"Provisioning a new Conda environment ({environmentPath}).");
            await ExecuteProcessAsync("conda", $"create -y -p {environmentPath} python={pythonVersion}", token);

            foreach (var dependency in condaDependencies)
            {
                if (string.IsNullOrWhiteSpace(dependency)) continue;

                Console.WriteLine($"Installing Conda dependency '{dependency}' into the Conda environment ({environmentPath}).");
                await ExecuteProcessAsync("conda", $"install {dependency} -c conda-forge -y -p {environmentPath}", token);
            }

            foreach (var dependency in pipDependencies)
            {
                if (string.IsNullOrWhiteSpace(dependency)) continue;

                Console.WriteLine($"Installing PIP dependency '{dependency}' into the Conda environment ({environmentPath}).");
                await ExecuteProcessAsync(pythonExecutablePath, $"-m pip install {dependency}", token);
            }
        }
        else
        {
            Console.WriteLine($"An environment with the required Python version and dependencies already exists. Using it ({environmentPath}).");
        }

        return pythonExecutablePath;
    }

    /// <summary>
    /// Generate a hash for a given python version and pip dependencies.
    /// </summary>
    /// <param name="pythonVersion">The version of Python to execute code against. For example '3.8'.</param>
    /// <param name="pipDependencies">A collection of PIP dependencies the Python code requires. The items can either be a package name (Example: 'pandas==1.2.3') or a package name with its version (Example: 'pandas').</param>
    /// <param name="condaDependencies">A collection of Conda dependencies the Python code requires. The items can either be a package name (Example: 'ffmpeg==1.2.3') or a package name with its version (Example: 'ffmpeg').</param>
    /// <returns>a Hash for a given python version and pip dependencies.</returns>
    private string GetPythonEnvironmentHash(string pythonVersion, List<string> pipDependencies, List<string> condaDependencies)
    {
        // Combine pythonVersion and pipDependencies into one string.
        var sb = new StringBuilder();

        foreach (string dep in pipDependencies.Order())
        {
            sb.Append(dep);
        }

        foreach (string dep in condaDependencies.Order())
        {
            sb.Append(dep);
        }

        // Convert the input string to a byte array and compute the hash.
        var data = SHA256
            .Create()
            .ComputeHash(Encoding.UTF8.GetBytes($"{pythonVersion}_{sb}"));

        // Create a new Stringbuilder to collect the bytes and create a string.
        var sBuilder = new StringBuilder();

        // Loop through each byte of the hashed data and format each one as a hexadecimal string.
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        // Return the hexadecimal string.
        return sBuilder.ToString();
    }

    /// <summary>
    /// Execute any terminal command silently and return the stringified response.
    /// </summary>
    /// <param name="fileName">The path of the process to execute. For example python, conda, c:/programf../python etc.</param>
    /// <param name="command">The command with all arguments in a string form.</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The stringifier response.</returns>
    private async Task<string> ExecuteProcessAsync(string fileName, string command, CancellationToken token)
    {
        var process = new Process();

        // If on Windows, use cmd.exe
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"/c {fileName} {command}";
        }
        else
        {
            if (fileName == "conda" && !process.StartInfo.Environment["PATH"].Contains("miniconda3"))
            {
                fileName = $"~/miniconda3/bin/{fileName}";
            }

            // If on macOS or Linux, use zsh
            process.StartInfo.FileName = "/bin/zsh";
            process.StartInfo.Arguments = $"-c \"{fileName} {command}\"";
        }

        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.Start();

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new ApplicationException($"An error occured during the execution of the Python code (see the inner exception).{Environment.NewLine}{Environment.NewLine}{command}", new Exception(error));
        }

        if (!string.IsNullOrWhiteSpace(error)) Console.WriteLine(error);
        Console.WriteLine(output);

        return output;
    }

    /// <summary>
    /// Execute any terminal command silently and return the stringified response.
    /// </summary>
    /// <param name="fileName">The path of the process to execute. For example python, conda, c:/programf../python etc.</param>
    /// <param name="code">The command with all arguments in a string form.</param>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The stringifier response.</returns>
    private async Task<string> ExecutePythonProcessAsync(string fileName, string code, CancellationToken token)
    {
        var process = new Process();
        var tempScriptFilePath = $"{Guid.NewGuid()}.py";

        await File.WriteAllTextAsync(tempScriptFilePath, code, token);

        // If on Windows, use cmd.exe.
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = $"-c \"{fileName} {tempScriptFilePath}\"";
        }
        else
        {
            if (fileName == "conda" && !process.StartInfo.Environment["PATH"].Contains("miniconda3"))
            {
                fileName = $"~/miniconda3/bin/{fileName}";
            }

            // If on macOS or Linux, use zsh.
            process.StartInfo.FileName = "/bin/zsh";
            process.StartInfo.Arguments = $"-c \"{fileName} {tempScriptFilePath}\"";
        }

        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.Start();

        var output = await process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new ApplicationException($"An error occured during the execution of the Python code (see the inner exception).{Environment.NewLine}{Environment.NewLine}{code}", new Exception(error));
        }

        if (!string.IsNullOrWhiteSpace(error)) Console.WriteLine(error);
        Console.WriteLine(output);

        return output;
    }
}
