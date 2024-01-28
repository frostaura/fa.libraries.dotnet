using FrostAura.Libraries.Semantic.Core.Interfaces.Data;

namespace Semantic.Core.Tests.Data;

public class ConsoleUserAgentProxy : IUserProxyDataAccess
{
    public Task<string> AskUserAsync(string question, CancellationToken token)
    {
        Console.Write($"[QUESTION] {question}: ");

        return Task.FromResult(Console.ReadLine());
    }
}
