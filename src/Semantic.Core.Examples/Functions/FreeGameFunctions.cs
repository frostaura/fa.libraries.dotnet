using System.ComponentModel;
using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace Semantic.Core.Examples.Functions;

/// <summary>
/// Free games functions.
/// </summary>
public class FreeGameFunctions : BaseThought
{
    /// <summary>
    /// Overloaded constructor to provide dependencies.
    /// </summary>
    /// <param name="serviceProvider">The dependency service provider.</param>
    /// <param name="semanticKernelLanguageModels">A component for communicating with language models.</param>
    /// <param name="logger">Instance logger.</param>
    public FreeGameFunctions(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, ILogger<FreeGameFunctions> logger)
        : base(serviceProvider, semanticKernelLanguageModels, logger)
    { }

    /// <summary>
    /// Creates a new free game offer that is simplfied by providing default values for certain fields. The cost per bet for each game provided is used to calculate a combination of chip size and number of coins that match the cost per bet value. The free game offer uses currency levelling for bets. The cost per bet takes on the user's currency when the free game offer is awarded to a user. Currency levelling causes the cost per bet to take on a base currency which then converted to the user's currency.
    ///
    /// The base currency is configured for the gaming system, and the exchange rates are generally static, rounded values.
    /// For example, the betting model may dictate that a bet costs 20.00 decimal currency units.If currency levelling isn't used, a user playing in ZAR would then wager R20 per bet, and a user playing in USD will wager $20 per bet. If currency levelling is used, the wager per bet would have roughly the same monetary value so that the user with the weaker currency isn't disadvantaged.
    /// </summary>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The response body as a string.</returns>
    [KernelFunction, Description("Creates a new free game offer that is simplfied by providing default values for certain fields. The cost per bet for each game provided is used to calculate a combination of chip size and number of coins that match the cost per bet value. The free game offer uses currency levelling for bets. The cost per bet takes on the user's currency when the free game offer is awarded to a user. Currency levelling causes the cost per bet to take on a base currency which then converted to the user's currency.\n\nThe base currency is configured for the gaming system, and the exchange rates are generally static, rounded values.\n\nFor example, the betting model may dictate that a bet costs 20.00 decimal currency units. If currency levelling isn't used, a user playing in ZAR would then wager R20 per bet, and a user playing in USD will wager $20 per bet. If currency levelling is used, the wager per bet would have roughly the same monetary value so that the user with the weaker currency isn't disadvantaged.")]
    public async Task<string> AddFreeGameOfferByNearestCostAsync(
        [Description("int: The ProductId. This identifies the system containing the free game offer. This value is domain-specific and MUST NOT be made up. The user MUST provide this value / Should be asked to provide this value")] string productId,
        [Description("int: The default number of free game wagers that will be awarded to a user. This value may be changed when the free game offer is allocated. This value is domain-specific and MUST NOT be made up. The user MUST provide this value / Should be asked to provide this value.")] string defaultNumberOfGames,
        [Description("int: Uniquely identifies the game in conjunction with the ModuleId. Typically used to identify the graphical skin or client software used to play the game. This value is domain-specific and MUST NOT be made up. This value can be obtained from the list of supported free games.")] string clientId,
        [Description("int: Uniquely identifies the game in conjunction with the ClientId. Typically used to identify the game engine or rules. This value is domain-specific and MUST NOT be made up. This value can be obtained from the list of supported free games.")] string moduleId,
        CancellationToken token = default)
    {
        // True API implementation goes here.
        // For the sake of the POC I simply went with a single game but games[] can be done too.

        return "Your free game offer was successfully created!";
    }

    /// <summary>
    /// Get Simplified Free Game Supported Games Returns basic information of free games supported games like clientId, moduleId, game name, nearest cost etc.
    /// </summary>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The response body as a string.</returns>
    [KernelFunction, Description("Get Simplified Free Game Supported Games Returns basic information of free games supported games like clientId, moduleId, game name, nearest cost etc.")]
    public async Task<string> GetSimplifiedFreeGameSupportedGamesAsync(
        [Description("int: The ProductId. This identifies the system containing the free game offer. This value is domain-specific and MUST NOT be made up. The user MUST provide this value / Should be asked to provide this value.")] string productId,
        CancellationToken token = default)
    {
        // True API implementation goes here.
        // For the sake of the POC I simply went with a single game but games[] can be done too.

        return """
            {
              "paging": {
                "pageNumber": 5,
                "itemsPerPage": 20,
                "total": 720
              },
              "data": [
                {
                  "moduleId": 19596,
                  "clientId": 10001,
                  "gameName": "The Twisted Circus (Flash)",
                  "clientTypeDescription": "HTML5 Desktop Client",
                  "clientTypeId": 1202,
                  "nearestCost": [
                    5.0,
                    6.0,
                    7.0
                  ]
                },
                {
                  "moduleId": 19626,
                  "clientId": 40300,
                  "gameName": "Thunderstruck II Scratch(HTML5-Mobile)",
                  "clientTypeDescription": "HTML5 Desktop Client",
                  "clientTypeId": 1202,
                  "nearestCost": [
                    5.0,
                    6.0,
                    7.0
                  ]
                }
              ]
            }
            """;
    }
}
