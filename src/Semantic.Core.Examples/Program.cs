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
using FrostAura.Libraries.Semantic.Core.Models.Prompts;
using FrostAura.Libraries.Semantic.Core.Thoughts.Cognitive;
using Microsoft.Extensions.DependencyInjection;
using Semantic.Core.Examples.Data;
using Semantic.Core.Examples.Functions;

// Create dependency injection service collection (standard practice).
var services = new ServiceCollection()
    // Add the core semantic libraries. This adds all services and bootstraps configuration.
    .AddSemanticCore(out var configuration)
    // Add any **additional** functions you would like the LLM to have access to. This is where you can provide custom or domain-specific functions.
    .AddSemanticThoughts(Assembly.GetAssembly(typeof(ExampleCustomFunctions)))
    /* We always need an implementation of 'IUserProxyDataAccess' to allow for the language models to communicate back and forth with the user (For example ask a clarifying question).
     * This implementation differs based on your client (Getting user feedback from a web app is different than from a console app etc).
     */
    .AddSingleton<IUserProxyDataAccess, ConsoleUserAgentProxy>()
    .BuildServiceProvider();
// Grab an instance of our language model via the services.
var llmThought = services
    .GetThoughtByName<LanguageModelThoughts>(nameof(LanguageModelThoughts));
// A cancellation token of your choice for allowing cancelling downstream operations. You could use a CancellationTokenSource to create your own where needed.
var token = CancellationToken.None;

// Start off a conversation by getting a query from the user and passing it to the language model.
Console.Write("Question / Query: ");

var query = Console.ReadLine();
var operationId = 1;
var operationContext = new OperationContext
{
    Id = $"{operationId}",
    Name = $"Operation {operationId}"
};
var conversation = await llmThought
    .ChatAsync(query, ModelType.LargeLLM, token, operationContext);

// Finally, simply create an infinite while loop to print the previous language model response and allow for a follow-up query.
while(true)
{
    operationId += 1;
    operationContext = new OperationContext
    {
        Id = $"{operationId}",
        Name = $"Operation {operationId}"
    };
    Console.WriteLine($"{Environment.NewLine}{conversation.LastMessage}");
    Console.Write($"{Environment.NewLine}{Environment.NewLine}Question / Query (Follow-Up): ");
    await conversation.ChatAsync(Console.ReadLine(), token, operationContext);
}