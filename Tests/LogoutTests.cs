using NUnit.Framework;
using OpenQA.Selenium;
using Selenium.Framework;
using Selenium.Framework.Models;
using Selenium.Pages;
using static Selenium.Pages.BasePage;

namespace Selenium.Tests
{
    public class LogoutTests : BaseTest
    {
        private UserModel user;

        [SetUp]
        protected void Initialize()
        {
            user = UserModel.GetDefaultUser();
        }

        [Test]
        public void Logout_In_New_Tab_Invalidates_First_Tab_Session()
        {
            var homePage = SiteNavigator.NavigateToLoginPage(Driver).Login(user);
            WaitHelper.WaitUntil(Driver, d => homePage.OnHeader().GetWelcomeText.Contains(user.FirstName));

            var firstTab = Driver.CurrentWindowHandle;

            Driver.SwitchTo().NewWindow(WindowType.Tab);
            SiteNavigator.NavigateToHomePage(Driver).Logout(user);
            WaitHelper.WaitUntil(Driver, d => d.Url.Contains("/login"));

            Driver.Close();
            Driver.SwitchTo().Window(firstTab);

            var allLink = WaitHelper.WaitForElementClickable(Driver, By.LinkText("All"));
            allLink.Click();

            WaitHelper.WaitUntil(Driver, d => d.Url.Contains("/login"), timeoutSeconds: 10);
            Assert.That(Driver.Url, Does.Contain("/login"));
        }
    }
}
