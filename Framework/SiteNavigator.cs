using OpenQA.Selenium;
using Selenium.Pages;

namespace Selenium.Framework
{
    public class SiteNavigator
    {


        public static LoginPage NavigateToLoginPage(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(Settings.BuildUrl("/auth/login"));
            return new LoginPage(driver);
        }

        public static HomePage NavigateToHomePage(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(Settings.BuildUrl(""));
            return new HomePage(driver);
        }

        public static RegistrationPage NavigateToRegisterPage(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(Settings.BuildUrl("/register"));
            return new RegistrationPage(driver);
        }

        public static MyApplicationsPage NavigateToMyApplicationsPage(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(Settings.BuildUrl("/my"));
            return new MyApplicationsPage(driver);
        }

        public static NewApplicationPage NavigateToNewApplicationPage(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(Settings.BuildUrl("/new"));
            return new NewApplicationPage(driver);
        }

        public static AJAXPage NavigateToAJAXPage(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(Settings.BuildUrl("/calc"));
            return new AJAXPage(driver);
        }

        public static JSPage NavigateToJSPage(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(Settings.BuildUrl("/js"));
            return new JSPage(driver);
        }
    }
}
