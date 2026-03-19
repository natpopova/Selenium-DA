using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using Selenium.Framework;
using Selenium.Framework.Models;
using Selenium.Pages;
using System;
using System.Diagnostics.Metrics;
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
        public void AJAXTestSetUp()
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
            const string firstNumber = "7";
            const string secondNumber = "1";
            const string expectedResult = "8";


            var ajaxPage = SiteNavigator.NavigateToAJAXPage(Driver);
            ajaxPage.EnterFirstNumber(firstNumber);
            ajaxPage.EnterSecondNumber(secondNumber);

            // Act: Нажимаем кнопку "Sum" и ждём результат.
            ajaxPage.ClickSumButton();
            string result = ajaxPage.WaitForResultTextContains(expectedResult);

            // Assert: Проверяем, что результат равен сумме двух чисел.
            Assert.That(result, Does.Contain(expectedResult), "The result does not contain the expected value.");
        }

        [Test]
        //Enter one valid number and one string (not a number), click ‘Sum’, wait for the result, and verify that the message ‘Incorrect data’ appears.
        public void AJAX_Sum_InvalidInput_ShouldShowErrorMessage()
        {
            // Arrange: Открываем страницу AJAX и вводим одно валидное число и одну строку.
            const string validNumber = "5";
            const string invalidInput = "abc";
            const string InvalidInputMessage = "Incorrect data";

            var ajaxPage = SiteNavigator.NavigateToAJAXPage(Driver);
            ajaxPage.EnterFirstNumber(validNumber);
            ajaxPage.EnterSecondNumber(invalidInput);

            // Act: Нажимаем кнопку "Sum" и ждём результат.
            ajaxPage.ClickSumButton();
            string errorMessage = ajaxPage.WaitForResultTextContains(InvalidInputMessage);

            Assert.That(errorMessage, Does.Contain(InvalidInputMessage), "The error message does not contain the expected text.");



        }
    }
}
