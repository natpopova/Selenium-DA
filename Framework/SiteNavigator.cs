using OpenQA.Selenium;
using Selenium.Pages;

namespace Selenium.Framework
{
    public class SiteNavigator
    {
        // Формируем абсолютный URL, чтобы не дублировать логику склейки путей.
        private static string BuildUrl(string relativePath)
        {
            var baseUrl = Settings.GetBaseUrl().TrimEnd('/');
            return baseUrl + relativePath;
        }

        public static LoginPage NavigateToLoginPage(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(BuildUrl("/auth/login"));
            return new LoginPage(driver);
        }

        public static HomePage NavigateToHomePage(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(BuildUrl(""));
            return new HomePage(driver);
        }

        public static RegistrationPage NavigateToRegisterPage(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(BuildUrl("/register"));
            return new RegistrationPage(driver);
        }

        public static MyApplicationsPage NavigateToMyApplicationsPage(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(BuildUrl("/my"));
            return new MyApplicationsPage(driver);
        }

        public static NewApplicationPage NavigateToNewApplicationPage(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(BuildUrl("/new"));
            return new NewApplicationPage(driver);
        }
    }
}
