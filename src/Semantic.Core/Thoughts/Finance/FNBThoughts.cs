using System.ComponentModel;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Core.Extensions.Resilience;
using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using OpenQA.Selenium;
using FrostAura.Libraries.Semantic.Core.Interfaces.Data;
using FrostAura.Libraries.Semantic.Core.Abstractions.Thoughts;
using FrostAura.Libraries.Semantic.Core.Thoughts.IO;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.Finance;

/// <summary>
/// Web driver thoughts.
/// </summary>
public class FNBThoughts : BaseThought
{
    /// <summary>
    /// FNB config.
    /// </summary>
    private readonly FNBConfig _fnbConfig;

    /// <summary>
    /// Overloaded constructor to provide dependencies.
    /// </summary>
    /// <param name="serviceProvider">The dependency service provider.</param>
    /// <param name="semanticKernelLanguageModels">A component for communicating with language models.</param>
    /// <param name="fnbOptions">FNB config options.</param>
    /// <param name="logger">Instance logger.</param>
    public FNBThoughts(IServiceProvider serviceProvider, ISemanticKernelLanguageModelsDataAccess semanticKernelLanguageModels, IOptions<FNBConfig> fnbOptions, ILogger<FNBThoughts> logger)
        : base(serviceProvider, semanticKernelLanguageModels, logger)
    {
        _fnbConfig = fnbOptions
            .ThrowIfNull(nameof(fnbOptions))
            .Value
            .ThrowIfNull(nameof(fnbOptions));
    }

    /// <summary>
    /// Fetch the FNB account balances in text format.
    /// </summary>
    /// <param name="token">The token to use to request cancellation.</param>
    /// <returns>The FNB account balances page HTML inner text.</returns>
    [KernelFunction, Description("Fetch the FNB account balances in text format.")]
    public Task<string> GetFNBAccountBalancesRawAsync(CancellationToken token = default)
    {
        return Task.Run(() =>
        {
            using (_logger.BeginScope("{MethodName}", nameof(GetFNBAccountBalancesRawAsync)))
            {
                var webDriverThough = _serviceProvider
                    .GetThoughtByName<WebDriverThoughts>(nameof(WebDriverThoughts));

                _logger.LogInformation("Getting FNB Accounts for user {Username}.", _fnbConfig.Username);

                webDriverThough.OnPageLoadedAsync = async (driver) => await NavigateToFNBAccountBalancesAsync(
                    driver,
                    _fnbConfig.Username,
                    _fnbConfig.Password
                );
                webDriverThough.OnCleanupAsync = OnDoneWithDriverAsync;

                return webDriverThough.LoadWebsiteTextAsync("https://fnb.co.za", token);
            }
        })
        .AsResilientTask();
    }

    /// <summary>
    /// A handler for when a page is done being navigated to
    /// </summary>
    /// <param name="driver">The Selenium web driver with the base page loaded.</param>
    /// <param name="username">FNB username.</param>
    /// <param name="password">FNB password.</param>
    private async Task NavigateToFNBAccountBalancesAsync(WebDriver driver, string username, string password)
    {
        using (_logger.BeginScope("{MethodName}", nameof(NavigateToFNBAccountBalancesAsync)))
        {
            _logger.LogInformation("Filling in the login form for {Username}.", username);

            var usernameInput = driver.FindElement(By.CssSelector("input#user"));
            var passwordInput = driver.FindElement(By.CssSelector("input#pass"));
            var submitButton = driver.FindElement(By.CssSelector("input#OBSubmit"));

            usernameInput.SendKeys(username);
            passwordInput.SendKeys(password);
            submitButton.Click();

            // Wait for the navigation to have occured before moving on.
            await Task.Delay(10000);

            var elementToClick = driver.FindElement(By.XPath($"//*[contains(text(), 'Accounts')]"));

            _logger.LogInformation($"Logging in.");
            elementToClick.Click();

            // Wait for the navigation to have occured before moving on.
            await Task.Delay(2500);
        }
    }

    /// <summary>
    /// A handler for when a page is done being navigated to
    /// </summary>
    /// <param name="driver">The Selenium web driver with the base page loaded.</param>
    private Task OnDoneWithDriverAsync(WebDriver driver)
    {
        using (_logger.BeginScope("{MethodName}", nameof(OnDoneWithDriverAsync)))
        {
            var logoutButton = driver.FindElement(By.CssSelector(".headerButton"));

            _logger.LogInformation($"Logging out.");
            logoutButton.Click();

            return Task.CompletedTask;
        }
    }
}