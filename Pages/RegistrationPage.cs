using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using Selenium.Framework.Models;

namespace Selenium.Pages
{
    public class RegistrationPage : BasePage
    {
        // Локаторы (централизовано, чтобы не дублировать строки)
        private static readonly By _login = By.Name("name");
        private static readonly By _firstName = By.Name("fname");
        private static readonly By _lastName = By.Name("lname");
        private static readonly By _password = By.Name("password");
        private static readonly By _confirmPassword = By.Name("passwordConfirm");
        private static readonly By _role = By.Name("role");
        private static readonly By _registerBtn = By.XPath("//input[@value='Register']");

        public RegistrationPage(IWebDriver driver) : base(driver)
        {
        }

        // Свойства для совместимости (каждый раз получаем элемент — не кешируем)
        public IWebElement NameBox => Driver.FindElement(_login);
        public IWebElement FirstNameBox => Driver.FindElement(_firstName);
        public IWebElement LastNameBox => Driver.FindElement(_lastName);
        public IWebElement PasswordBox => Driver.FindElement(_password);
        public IWebElement ConfirmPasswordBox => Driver.FindElement(_confirmPassword);
        public IWebElement RoleDropdown => Driver.FindElement(_role);
        public IWebElement RegisterButton => Driver.FindElement(_registerBtn);

        // Тип роли (можно расширять при появлении новых значений)
        public enum RoleType
        {
            Developer,
            User
        }

        private SelectElement RoleSelect => new SelectElement(WaitHelper.WaitForElementVisible(Driver, _role));

        // Метод выбора роли из выпадающего списка
        public void SelectRole(RoleType role)
        {
            string value = role == RoleType.Developer ? "DEVELOPER" : "USER";
            RoleSelect.SelectByValue(value);
        }


        public void FillForm(UserModel user, RoleType role)
        {
            // Используем явные ожидания для стабильности
            WaitHelper.WaitForElementVisible(Driver, _login).Clear();
            NameBox.SendKeys(user.Login);

            WaitHelper.WaitForElementVisible(Driver, _firstName).Clear();
            FirstNameBox.SendKeys(user.FirstName);

            WaitHelper.WaitForElementVisible(Driver, _lastName).Clear();
            LastNameBox.SendKeys(user.LastName);

            WaitHelper.WaitForElementVisible(Driver, _password).Clear();
            PasswordBox.SendKeys(user.Password);

            WaitHelper.WaitForElementVisible(Driver, _confirmPassword).Clear();
            user.ConfirmPassword = user.Password;
            ConfirmPasswordBox.SendKeys(user.ConfirmPassword);

            SelectRole(role);
        }

        public void Submit()
        {
            WaitHelper.WaitForElementClickable(Driver, _registerBtn).Click();
        }

        public void Register(UserModel user, RoleType role)
        {
            FillForm(user, role);
            Submit();
        }
    }
}
