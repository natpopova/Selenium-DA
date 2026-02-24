using OpenQA.Selenium;
using Selenium.Framework.Models;

namespace Selenium.Pages
{
    public class HomePage : BasePage
    {
        // Локаторы ссылок в верхнем меню.
        private readonly By _logoutLink = By.PartialLinkText("Logout");
        private readonly By _allLink = By.PartialLinkText("All");

        public HomePage(IWebDriver driver) : base(driver)
        {
        }

        // Получаем ссылку Logout.
        public IWebElement LogoutLink => Driver.FindElement(_logoutLink);

        // Получаем ссылку All.
        public IWebElement AllLink => Driver.FindElement(_allLink);


        // Выходим из аккаунта и возвращаем страницу логина.
        public LoginPage Logout(UserModel user)
        {
            // Параметр user оставлен для совместимости со старыми тестами.
            LogoutLink.Click();
            return new LoginPage(Driver);
        }
    }
}
