using System;
using OpenQA.Selenium;
using Selenium.Framework.Models;

namespace Selenium.Pages
{
    public class HomePage : BasePage
    {
        public HomePage(IWebDriver driver) : base(driver)
        {
        }

        public IWebElement LogoutLink => Driver.FindElement(By.PartialLinkText("Logout"));
        public IWebElement AllLink => Driver.FindElement(By.PartialLinkText("All"));

    
        public LoginPage Logout(UserModel user)
        {
            LogoutLink.Click();
            return new LoginPage(Driver);
        }


    }
}