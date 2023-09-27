using System.ComponentModel;
using FrostAura.Libraries.Core.Extensions.Validation;
using FrostAura.Libraries.Semantic.Core.Extensions.Configuration;
using FrostAura.Libraries.Semantic.Core.Models.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel.SkillDefinition;
using OpenQA.Selenium;

namespace FrostAura.Libraries.Semantic.Core.Thoughts.IO
{
    /// <summary>
    /// Web driver thoughts.
    /// </summary>
    public class FNBThoughts : BaseThought
    {
        /// <summary>
        /// The dependency service provider.
        /// </summary>
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// FNB config.
        /// </summary>
        private readonly FNBConfig _fnbConfig;

        /// <summary>
        /// Overloaded constructor to provide dependencies.
        /// </summary>
        /// <param name="serviceProvider">The dependency service provider.</param>
        /// <param name="fnbOptions">FNB config options.</param>
        /// <param name="logger">Instance logger.</param>
        public FNBThoughts(IServiceProvider serviceProvider, IOptions<FNBConfig> fnbOptions, ILogger<FNBThoughts> logger)
            : base(logger)
        {
            _serviceProvider = serviceProvider
                .ThrowIfNull(nameof(serviceProvider));
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
        [SKFunction, Description("Fetch the FNB account balances in text format.")]
        public Task<string> GetFNBAccountBalancesAsync(CancellationToken token = default)
        {
            var webDriverThough = (WebDriverThoughts)_serviceProvider
                .GetThoughtByName(nameof(WebDriverThoughts));

            webDriverThough.OnPageLoadedAsync = async (driver) => await NavigateToFNBAccountBalancesAsync(
                driver,
                _fnbConfig.Username,
                _fnbConfig.Password
            );
            webDriverThough.OnCleanupAsync = OnDoneWithDriverAsync;

            return webDriverThough.LoadTextAsync("https://fnb.co.za", token);
        }

        /// <summary>
        /// A handler for when a page is done being navigated to
        /// </summary>
        /// <param name="driver">The Selenium web driver with the base page loaded.</param>
        /// <param name="username">FNB username.</param>
        /// <param name="password">FNB password.</param>
        private async Task NavigateToFNBAccountBalancesAsync(WebDriver driver, string username, string password)
    {
        var usernameInput = driver.FindElement(By.CssSelector("input#user"));
        var passwordInput = driver.FindElement(By.CssSelector("input#pass"));
        var submitButton = driver.FindElement(By.CssSelector("input#OBSubmit"));

        usernameInput.SendKeys(username);
        passwordInput.SendKeys(password);
        submitButton.Click();

        // Wait for the navigation to have occured before moving on.
        await Task.Delay(10000);

        var elementToClick = driver.FindElement(By.XPath($"//*[contains(text(), 'Accounts')]"));

        elementToClick.Click();

        // Wait for the navigation to have occured before moving on.
        await Task.Delay(2500);
    }

        /// <summary>
        /// A handler for when a page is done being navigated to
        /// </summary>
        /// <param name="driver">The Selenium web driver with the base page loaded.</param>
        private Task OnDoneWithDriverAsync(WebDriver driver)
        {
            var logoutButton = driver.FindElement(By.CssSelector(".headerButton"));

            logoutButton.Click();

            return Task.CompletedTask;
        }
    }
}