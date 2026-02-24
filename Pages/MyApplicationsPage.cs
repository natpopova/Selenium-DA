using OpenQA.Selenium;

namespace Selenium.Pages
{
    public class MyApplicationsPage : BasePage
    {
        // Локатор ссылки для добавления нового приложения.
        private readonly By _addNewApplicationLink = By.PartialLinkText("new");

        public MyApplicationsPage(IWebDriver driver) : base(driver)
        {
        }

        // Возвращаем ссылку "Add new application".
        public IWebElement AddNewApplicationLink => Driver.FindElement(_addNewApplicationLink);

        // Открываем форму создания нового приложения.
        public NewApplicationPage OpenNewApplicationForm()
        {
            AddNewApplicationLink.Click();
            return new NewApplicationPage(Driver);
        }
    }
}
