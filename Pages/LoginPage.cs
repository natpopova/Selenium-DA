using OpenQA.Selenium;
using Selenium.Framework.Models;

namespace Selenium.Pages
{
    public class LoginPage : BasePage
    {
        // Локаторы элементов формы логина.
        private readonly By _usernameBox = By.Id("j_username");
        private readonly By _passwordBox = By.Id("j_password");
        private readonly By _loginButton = By.XPath("//input[@value='Login']");
        private readonly By _registerLink = By.PartialLinkText("Register");

        public LoginPage(IWebDriver driver) : base(driver)
        {
        }

        // Элемент поля логина.
        public IWebElement UsernameBox => Driver.FindElement(_usernameBox);

        // Элемент поля пароля.
        public IWebElement PasswordBox => Driver.FindElement(_passwordBox);

        // Кнопка входа.
        public IWebElement LoginButton => Driver.FindElement(_loginButton);

        // Ссылка на регистрацию.
        public IWebElement RegisterLink => Driver.FindElement(_registerLink);


        // Выполняем вход под пользователем и возвращаем домашнюю страницу.
        public HomePage Login(UserModel user)
        {
            // Заполняем поле логина.
            UsernameBox.SendKeys(user.Login);

            // Заполняем поле пароля.
            PasswordBox.SendKeys(user.Password);

            // Нажимаем кнопку входа.
            LoginButton.Click();

            return new HomePage(Driver);
        }
    }
}
