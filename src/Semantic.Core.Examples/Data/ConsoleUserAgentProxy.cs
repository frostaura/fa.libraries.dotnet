﻿using FrostAura.Libraries.Semantic.Core.Enums.Logging;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Models.Logging;

namespace Semantic.Core.Examples.Data;

/// <summary>
/// A console proxy service requirements for a user proxy data.
/// </summary>
public class ConsoleUserAgentProxy : IUserProxyDataAccess
{
    /// <summary>
    /// Ask the user proxy a question and get an instance of the completion source for when the question has been answered.
    /// </summary>
    /// <param name="question">Question to ask the user proxy.</param>
    /// <param name="token">Token to cancel downstream operations.</param>
    /// <returns>The answer from the user.</returns>
    public Task<string> AskUserAsync(string question, CancellationToken token)
    {
        Console.WriteLine($"{question}");
        Console.Write("Answer: ");

        return Task.FromResult(Console.ReadLine());
    }

    /// <summary>
    /// A handler for when a system event occurs.
    /// </summary>
    /// <param name="scopes">A collection of all the scopes in the event system.</param>
    /// <param name="currentEvent">The current occurance / most recent event.</param>
    /// <returns>Void</returns>
    public Task OnEventAsync(List<LogItem> scopes, LogItem currentEvent)
    {
        var formattedLog = scopes
            .Where(s => s.Scope == default)
            .Select(s => GetEventTreeLogRecursively(scopes, s))
            .Aggregate((l, r) => $"{l}{Environment.NewLine}{r}");

        Console.Clear();
        Console.WriteLine(formattedLog);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Recursively process events and draw a hierarchy based on their parent ids.
    /// </summary>
    /// <param name="scopes">A collection of all the scopes in the event system.</param>
    /// <param name="item">The current event.</param>
    /// <param name="index">Depth in the hierarchy.</param>
    /// <returns>The string representation.</returns>
    private string GetEventTreeLogRecursively(List<LogItem> scopes, LogItem item, int index = 1)
    {
        // Root-level log.
        var delimiter = "+";
        var response = item.Type == LogType.ScopeRoot ? Enumerable
                .Range(1, index)
                .Select(i => delimiter)
                .Aggregate((l, r) => $"{l}{r}") + $" {item.Message}" + Environment.NewLine : string.Empty;
        var lineItems = new List<(DateTime timestamp, string message)>();

        // Write direct child logs.
        foreach (var childItem in item.Logs)
        {
            var childResponse = Enumerable
                .Range(1, index + 1)
                .Select(i => delimiter)
                .Aggregate((l, r) => $"{l}{r}") + $" {childItem.Message}" + Environment.NewLine;

            // Write all direct children.
            foreach (var nestedChildItem in childItem.Logs)
            {
                childResponse += GetEventTreeLogRecursively(scopes, nestedChildItem, index + 1);
            }

            lineItems.Add((childItem.Timestamp, childResponse));
        }

        // Write all dependent items.
        var dependents = scopes
            .Where(s => s.Scope?.Id == item.Id);

        foreach (var dependentItem in dependents)
        {
            var dependentResponse = GetEventTreeLogRecursively(scopes, dependentItem, index + 1);

            lineItems.Add((dependentItem.Timestamp, dependentResponse));
        }

        return lineItems
            .OrderBy(li => li.timestamp)
            .Select(li => li.message)
            .Aggregate((l, r) => $"{l}{r}");
    }
}
