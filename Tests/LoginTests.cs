using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using Selenium.Framework;
using Selenium.Framework.Models;
using Selenium.Pages;
using static Selenium.Pages.BasePage;

namespace Selenium.Tests
{
    public class LoginTests : BaseTest
    {
        private const string InvalidLoginMessage = "invalid username or password";
        private UserModel user;

        [SetUp]
        protected void Initialize()
        {
            user = UserModel.GetDefaultUser();
        }

        [Test]
        public void Login_As_Valid_User()
        {
            var homePage = SiteNavigator.NavigateToLoginPage(Driver).Login(user);

            Logger.Info("Assert user login");
            WaitHelper.WaitUntil(Driver, d => homePage.OnHeader().GetWelcomeText.Contains(user.FirstName));
            ClassicAssert.True(homePage.OnHeader().GetWelcomeText.Contains(user.FirstName));
        }

        [Test]
        public void Login_As_Valid_User_With_Incorrect_Password()
        {
            user.Password = "invalid";

            var loginPage = SiteNavigator.NavigateToLoginPage(Driver);
            loginPage.Login(user);

            WaitHelper.WaitUntil(Driver, d => loginPage.GetFlashMessage().Contains(InvalidLoginMessage));
            ClassicAssert.True(loginPage.GetFlashMessage().Contains(InvalidLoginMessage));
            Assert.That(Driver.Url, Does.Contain("/login"));
        }
    }
}
