using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Pages
{
    public class Header : BasePage  // Header - часть страницы и её наследник
    {
        public Header(IWebDriver driver) : base(driver)
        {
        }

        public IWebElement WelcomeLabel => Driver.FindElement(By.CssSelector(".welcome"));
        public IWebElement Title => Driver.FindElement(By.CssSelector(".header .title"));
        public IWebElement NavAjax => Driver.FindElement(By.LinkText("Ajax test page"));
        public IWebElement NavJs => Driver.FindElement(By.LinkText("JS test page"));
        public IWebElement NavMyApplications => Driver.FindElement(By.LinkText("My applications"));
        public IWebElement EditAccount => Driver.FindElement(By.LinkText("Edit account"));
        public IWebElement HomeLink => Driver.FindElement(By.LinkText("Home"));
        public IWebElement LogOutLink => Driver.FindElement(By.LinkText("Logout"));

        #region Methods

        public string GetWelcomeText => WelcomeLabel.Text;
        public string GetTitleText => Title.Text;

        public LoginPage Logout()
        {
            LogOutLink.Click();
            return new LoginPage(Driver);
        }
        #endregion

        //  Методы навигации  (по элементам хэдэра) 

        public MyApplicationsPage GoToMyApplications()
        {
            NavMyApplications.Click();
            return new MyApplicationsPage(Driver);
        }

        public void GoToAjaxPage() => NavAjax.Click();
        public void GoToJsPage() => NavJs.Click();
        public void GoToHomePage() => HomeLink.Click();
        public void EditUserAccount() => EditAccount.Click();
        public void LogoutUser() => LogOutLink.Click();


    }


}