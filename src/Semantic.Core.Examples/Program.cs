/*
 * Nuget Dependencies:
 * -------------------
 * dotnet add package FrostAura.Libraries.Intelligence.Semantic.Core
 * dotnet add package Microsoft.SemanticKernel.Abstractions (For custom functions)
 * 
 * Configuration:
 * --------------
 * appsettings.json or via environment variables,
 * should provide the entire SemanticConfig object via JSON on a field called 'SemanticConfig' on the root of the JSON config object.
 */

using System.Reflection;
using FrostAura.Libraries.Semantic.Core.Enums.Models;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using Microsoft.Extensions.DependencyInjection;
using Semantic.Core.Examples;
using Semantic.Core.Examples.Data;
using Semantic.Core.Examples.Functions;

// Create dependency injection service collection.
var services = new ServiceCollection()
    .AddSemanticCore(out var configuration)
    .AddSemanticThoughts(Assembly.GetAssembly(typeof(ExampleCustomFunctions)))
    .AddSingleton<IUserProxyDataAccess, ConsoleUserAgentProxy>()
    .AddSingleton<Test1>()
    .AddSingleton<Test2>()
    .AddSingleton<Test3>()
    .BuildServiceProvider();
var llmThought = services
    .GetThoughtByName<LanguageModelThoughts>(nameof(LanguageModelThoughts));
var token = CancellationToken.None;

/*await services
    .GetRequiredService<Test1>()
    .UpdateAsync();*/


Console.Write("Question / Query: ");

var query = Console.ReadLine();
var conversation = await llmThought
    .ChatAsync(query, ModelType.LargeLLM, token);

while(true)
{
    Console.WriteLine($"{Environment.NewLine}{conversation.LastMessage}");
    Console.Write($"{Environment.NewLine}{Environment.NewLine}Question / Query (Follow-Up): ");
    await conversation.ChatAsync(Console.ReadLine(), token);
}