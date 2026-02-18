using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium.Support.UI;
using Selenium.Framework;       
using Selenium.Framework.Models;
using Selenium.Pages;
using static Selenium.Pages.BasePage;

namespace Selenium.Tests
{
    public class LoginTests : BaseTest
    {
        private UserModel user;

        [SetUp]
        protected void Initialize()
        {
            user = UserModel.GetDefaultUser();
        }

        [Test]
        public void ValidLoginTest()
        {
            HomePage homePage = SiteNavigator.NavigateToLoginPage(Driver).Login(user);
            Logger.Info("Assert user login");   
            WaitHelper.WaitUntil(Driver, d => homePage.OnHeader().GetWelcomeText.Contains(user.FirstName));
            ClassicAssert.True(homePage.OnHeader().GetWelcomeText.Contains(user.FirstName));
        }

        [Test]
        public void InvalidLoginTest()
        {
            user.Password = "invalid";

            LoginPage loginPage = SiteNavigator.NavigateToLoginPage(Driver);
            loginPage.Login(user);
            WaitHelper.WaitUntil(Driver, d => loginPage.GetFlashMessage().Contains("invalid username or password"));
            ClassicAssert.True(loginPage.GetFlashMessage().Contains("invalid username or password"));
        }
    }
}