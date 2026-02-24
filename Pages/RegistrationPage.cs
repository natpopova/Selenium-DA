using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Selenium.Framework.Models;

namespace Selenium.Pages
{
    public class RegistrationPage : BasePage
    {
        // Локаторы полей регистрации.
        private readonly By _login = By.Name("name");
        private readonly By _firstName = By.Name("fname");
        private readonly By _lastName = By.Name("lname");
        private readonly By _password = By.Name("password");
        private readonly By _confirmPassword = By.Name("passwordConfirm");
        private readonly By _role = By.Name("role");
        private readonly By _registerButton = By.XPath("//input[@value='Register']");

        public RegistrationPage(IWebDriver driver) : base(driver)
        {
        }

        // Поле логина.
        public IWebElement NameBox
        {
            get { return Driver.FindElement(_login); }
        }

        // Поле имени.
        public IWebElement FirstNameBox
        {
            get { return Driver.FindElement(_firstName); }
        }

        // Поле фамилии.
        public IWebElement LastNameBox
        {
            get { return Driver.FindElement(_lastName); }
        }

        // Поле пароля.
        public IWebElement PasswordBox
        {
            get { return Driver.FindElement(_password); }
        }

        // Поле подтверждения пароля.
        public IWebElement ConfirmPasswordBox
        {
            get { return Driver.FindElement(_confirmPassword); }
        }

        // Выпадающий список роли.
        public IWebElement RoleDropdown
        {
            get { return Driver.FindElement(_role); }
        }

        // Кнопка регистрации.
        public IWebElement RegisterButton
        {
            get { return Driver.FindElement(_registerButton); }
        }

        public enum RoleType
        {
            USER,
            DEVELOPER
        }

        // Заполняем форму регистрации и отправляем её.
        public LoginPage Register(UserModel user, RoleType roleType = RoleType.USER)
        {
            // Вводим логин.
            NameBox.Clear();
            NameBox.SendKeys(user.Login);

            // Вводим имя.
            FirstNameBox.Clear();
            FirstNameBox.SendKeys(user.FirstName);

            // Вводим фамилию.
            LastNameBox.Clear();
            LastNameBox.SendKeys(user.LastName);

            // Вводим пароль.
            PasswordBox.Clear();
            PasswordBox.SendKeys(user.Password);

            // Повторяем пароль для проверки.
            ConfirmPasswordBox.Clear();
            ConfirmPasswordBox.SendKeys(user.Password);

            // Выбираем роль пользователя в выпадающем списке.
            var roleSelect = new SelectElement(RoleDropdown);
            roleSelect.SelectByText(roleType == RoleType.DEVELOPER ? "DEVELOPER" : "USER");

            // Отправляем форму.
            RegisterButton.Click();

            // После успешной регистрации система возвращает на страницу входа.
            return new LoginPage(Driver);
        }
    }
}
