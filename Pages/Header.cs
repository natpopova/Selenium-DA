using OpenQA.Selenium;

namespace Selenium.Pages
{
    public class Header : BasePage
    {
        // Локаторы элементов шапки сайта.
        private readonly By _welcomeLabel = By.CssSelector(".welcome");
        private readonly By _title = By.CssSelector(".header .title");
        private readonly By _navAjax = By.LinkText("Ajax test page");
        private readonly By _navJs = By.LinkText("JS test page");
        private readonly By _navMyApplications = By.LinkText("My applications");
        private readonly By _editAccount = By.LinkText("Edit account");
        private readonly By _homeLink = By.LinkText("Home");
        private readonly By _logOutLink = By.LinkText("Logout");

        public Header(IWebDriver driver) : base(driver)
        {
        }

        // Элементы шапки.
        public IWebElement WelcomeLabel { get { return Driver.FindElement(_welcomeLabel); } }
        public IWebElement Title { get { return Driver.FindElement(_title); } }
        public IWebElement NavAjax { get { return Driver.FindElement(_navAjax); } }
        public IWebElement NavJs { get { return Driver.FindElement(_navJs); } }
        public IWebElement NavMyApplications { get { return Driver.FindElement(_navMyApplications); } }
        public IWebElement EditAccount { get { return Driver.FindElement(_editAccount); } }
        public IWebElement HomeLink { get { return Driver.FindElement(_homeLink); } }
        public IWebElement LogOutLink { get { return Driver.FindElement(_logOutLink); } }

        // Читаем приветствие пользователя.
        public string GetWelcomeText
        {
            get { return WelcomeLabel.Text; }
        }

        // Читаем заголовок в шапке.
        public string GetTitleText
        {
            get { return Title.Text; }
        }

        // Выходим из аккаунта и возвращаем страницу входа.
        public LoginPage Logout()
        {
            LogOutLink.Click();
            return new LoginPage(Driver);
        }

        // Переходим в раздел "My applications".
        public MyApplicationsPage GoToMyApplications()
        {
            NavMyApplications.Click();
            return new MyApplicationsPage(Driver);
        }

        // Переходим на страницу Ajax теста.
        public void GoToAjaxPage()
        {
            NavAjax.Click();
        }

        // Переходим на страницу JavaScript теста.
        public void GoToJsPage()
        {
            NavJs.Click();
        }

        // Переходим на главную.
        public void GoToHomePage()
        {
            HomeLink.Click();
        }

        // Открываем редактирование аккаунта.
        public void EditUserAccount()
        {
            EditAccount.Click();
        }

        // Альтернативный метод выхода без возврата нового объекта.
        public void LogoutUser()
        {
            LogOutLink.Click();
        }
    }
}
