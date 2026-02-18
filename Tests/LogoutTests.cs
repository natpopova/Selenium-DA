using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Selenium.Framework;
using Selenium.Framework.Models;
using Selenium.Pages;
using static Selenium.Pages.BasePage;
using SeleniumExtras.WaitHelpers;

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
        public void LogoutAfterValidLoginTest()
        {
            HomePage homePage = SiteNavigator.NavigateToLoginPage(Driver).Login(user);
            string firstTab = Driver.CurrentWindowHandle; // Сохраняем в строку идентификатор текущей вкладки - чтобы открыть её потом
            Logger.Info("Assert user login");
            Driver.SwitchTo().NewWindow(WindowType.Tab);    // Открыть новую вкладку
            SiteNavigator.NavigateToHomePage(Driver).Logout(user);    // идем на HomePage и вылогиниваемся
            Driver.Close();         // Закрываем вкладку с логаутом
            Driver.SwitchTo().Window(firstTab);         // Переключаемся обратно на первую вкладку
            By allLink = By.LinkText("All");
            //var clickable = wait.Until(ExpectedConditions.ElementToBeClickable(allLink));
            var clickable = WaitHelper.WaitForElementClickable(Driver, allLink, timeoutSeconds: 10);   // лучше код на 39 или 40 строке ???
            clickable.Click();
            WaitHelper.WaitUntil(Driver, d => d.Url.Contains("/login"), timeoutSeconds: 10);

            Assert.That(Driver.Url, Does.Contain("/login"));

        }

    }
}