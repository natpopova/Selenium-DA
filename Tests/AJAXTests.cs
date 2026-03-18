using System;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using Selenium.Framework;
using Selenium.Framework.Models;
using Selenium.Pages;
using static Selenium.Pages.BasePage;



namespace Selenium.Tests
{
    public class AJAXTests : BaseTest
    {
        // Константа с ожидаемым текстом ошибки для невалидного ввода.
        private const string InvalidInputMessage = "Incorrect data";
        // Поле для хранения тестового пользователя в рамках одного теста.
        private UserModel user;

        // SetUp-метод выполняется перед каждым тестом в этом классе.
        [SetUp]

        // Для AJAX-текста используем только explicit wait.
        public void AJAXSetUp()
        {
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;

            user = UserModel.GetDefaultUser();
            SiteNavigator.NavigateToLoginPage(Driver).Login(user);

        }


        // Enter two valid numbers, click ‘Sum’, wait for the result and check if the result is correct.
        [Test] //Это NUnit-атрибут, который помечает метод как тестовый кейс для выполнения тест-раннером.
        
        public void AJAX_Sum_ValidInput_ShouldReturnCorrectResult()
        {
            // Arrange: Открываем страницу AJAX и вводим два валидных числа.
            var ajaxPage = SiteNavigator.NavigateToAJAXPage(Driver);
            ajaxPage.EnterFirstNumber("7");
            ajaxPage.EnterSecondNumber("1");
            // Act: Нажимаем кнопку "Sum" и ждём результат.
            ajaxPage.ClickSumButton();
            int result = ajaxPage.WaitForResultTextContains();
            // Assert: Проверяем, что результат равен сумме двух чисел.
            Assert.AreEqual(num1 + num2, result, "The sum result is incorrect.");
        }

    }
}
