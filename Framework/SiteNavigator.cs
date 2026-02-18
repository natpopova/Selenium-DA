using System.Configuration;
using OpenQA.Selenium;
using Selenium.Pages;

namespace Selenium.Framework
{
    public class SiteNavigator
    {
        public static LoginPage NavigateToLoginPage(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(ConfigurationManager.AppSettings["baseUrl"] + "/auth/login");
            return new LoginPage(driver);
        }

        public static HomePage NavigateToHomePage(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(ConfigurationManager.AppSettings["baseUrl"]); 
            return new HomePage(driver);
        }

        public static RegistrationPage NavigateToRegisterPage(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(ConfigurationManager.AppSettings["baseUrl"] + "/register");
            return new RegistrationPage(driver);
        }

        public static MyApplicationsPage NavigateToMyApplicationsPage(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(ConfigurationManager.AppSettings["baseUrl"] + "/my");
            return new MyApplicationsPage(driver);
        }

        public static NewApplicationPage NavigateToNewApplicationPage(IWebDriver driver)
        {
            driver.Navigate().GoToUrl(ConfigurationManager.AppSettings["baseUrl"] + "/new");
            return new NewApplicationPage(driver);
        }
    }
}