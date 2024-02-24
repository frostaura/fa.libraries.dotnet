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
    [KernelFunction, Description("Execute Python code against a specific version of Python as well as with specific PIP and conda package requirements. NOTE: You must assume that no packages are pre-installed and provide all your dependencies.")]
    public async Task<string> InvokePythonAsync(
        [Description("The version of Python to execute code against. For example '3.8'.")] string pythonVersion,
        [Description("A collection of PIP dependencies the Python code requires. The items can either be a package name (Singular Example: 'pandas==1.2.3', Multiples Example: 'pandas==1.2.3 pip') or a package name with its version (Example: 'pandas').")] string pipDependencies,
        [Description("A collection of Conda dependencies the Python code requires. The items can either be a package name (Singular Example: 'ffmpeg==1.2.3', Multiples Example: 'ffmpeg==1.2.3 another-package==1.2.3') or a package name with its version (Example: 'ffmpeg').")] string condaDependencies,
        [Description(@"Python code to execute. The only requirement for the code block MUST contain a function called 'main' (def main() -> str:) that accepts zero arguments and returns a string. Example:
                    def some_code_you_generated() -> str:
                        return 'example output'
                    def main() -> str:
                        return some_code_you_generated()
            ")] string code,
        CancellationToken token = default)
    {
        using (_logger.BeginScope("{MethodName}", nameof(InvokePythonAsync)))
        {
            if (!code
            .ThrowIfNullOrWhitespace(nameof(code))
            .Contains("def main()"))
            {
                var exception = new ArgumentException(@"The code block MUST contain a function called 'main' (def main() -> str:) that accepts zero arguments and returns a string. When encountering tilde (~) in file names, you MUST expand it before using it (With os.path.expanduser(target_file) for example). Example:
                        def some_code_you_generated() -> str:
                            return 'example output'
                        def main() -> str:
                            return some_code_you_generated()
                ", nameof(code));

                _logger.LogError("The Python code block didn't contain a main function. Code: {Code}", code);
                throw exception;
            }

            _logger.LogInformation("Ensuring a Python {PythonVersion} env with {PipDependencies} PIP dependencies.", pythonVersion, pipDependencies);
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
            var response = await ExecutePythonProcessAsync(pythonExecutablePath, executableCode, token);
            var normalizedResponse = response
                .Split($"{outputIndicator}>>>")
                .Last()
                .Split($"<<<{outputIndicator}")
                .First();

            _logger.LogInformation("Successfully executed Python code. Response: {Response}", normalizedResponse);

            return normalizedResponse;
        }
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
            _logger.LogInformation("Provisioning a new Conda environment ({EnvironmentPath}).", environmentPath);
            await ExecuteProcessAsync("conda", $"create -y -p {environmentPath} python={pythonVersion}", token);

            foreach (var dependency in condaDependencies)
            {
                if (string.IsNullOrWhiteSpace(dependency)) continue;

                _logger.LogDebug("Installing Conda dependency '{Dependency}' into the Conda environment ({EnvironmentPath}).", dependency, environmentPath);
                await ExecuteProcessAsync("conda", $"install {dependency} -c conda-forge -y -p {environmentPath}", token);
            }

            foreach (var dependency in pipDependencies)
            {
                if (string.IsNullOrWhiteSpace(dependency)) continue;

                _logger.LogDebug("Installing PIP dependency '{Dependency}' into the Conda environment ({EnvironmentPath}).", dependency, environmentPath);
                await ExecuteProcessAsync(pythonExecutablePath, $"-m pip install {dependency}", token);
            }
        }
        else
        {
            _logger.LogInformation("An environment with the required Python version and dependencies already exists. Using it ({EnvironmentPath}).", environmentPath);
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
        using (_logger.BeginScope("{MethodName}", nameof(ExecuteProcessAsync)))
        {
            var process = new Process();

            // If on Windows, use cmd.exe
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                _logger.LogDebug("Identified Windows as the operating system.");

                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = $"/c {fileName} {command}";
            }
            else
            {
                _logger.LogDebug("Identified Linux-based operating system.");

                if (fileName == "conda" && !process.StartInfo.Environment["PATH"].Contains("miniconda3"))
                {
                    fileName = $"~/miniconda3/bin/{fileName}";
                }

                // If on macOS or Linux, use zsh
                process.StartInfo.FileName = "/bin/bash";
                process.StartInfo.Arguments = $"-c \"{fileName} {command}\"";
            }

            _logger.LogInformation("Starting process {FileName} {Arguments}", process.StartInfo.FileName, process.StartInfo.Arguments);

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();

            var output = await process.StandardOutput.ReadToEndAsync();
            var error = await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                var exception = new ApplicationException($"An error occured during the execution of the Python code (see the inner exception).{Environment.NewLine}{Environment.NewLine}{command}", new Exception(error));

                _logger.LogError("{Message}: {Exception}", exception.Message, exception);
                throw exception;
            }

            if (!string.IsNullOrWhiteSpace(error)) _logger.LogError(error);
            _logger.LogDebug(output);

            return output;
        }
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
        using (_logger.BeginScope("{MethodName}", nameof(ExecutePythonProcessAsync)))
        {
            var process = new Process();
            var tempScriptFilePath = $"{Guid.NewGuid()}.py";

            _logger.LogInformation("Saving code to execute to a local file '{TempScriptFilePath}'.", tempScriptFilePath);
            await File.WriteAllTextAsync(tempScriptFilePath, code, token);
            _logger.LogInformation("Executing terminal command: $\"-c \\\"{FileName} {TempScriptFilePath}\\\"\"", fileName, tempScriptFilePath);

            // If on Windows, use cmd.exe.
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                _logger.LogDebug("Identified Windows as the operating system.");

                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = $"-c \"{fileName} {tempScriptFilePath}\"";
            }
            else
            {
                _logger.LogDebug("Identified Linux-based operating system.");

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
                var exception = new ApplicationException($"An error occured during the execution of the Python code (see the inner exception).{Environment.NewLine}{Environment.NewLine}{code}{Environment.NewLine}Error: {error}", new Exception(error));

                _logger.LogError("{Message}: {Exception}", exception.Message, exception);
                throw exception;
            }

            if (!string.IsNullOrWhiteSpace(error)) _logger.LogError(error);
            _logger.LogDebug(output);

            return output;
        }
    }
}
