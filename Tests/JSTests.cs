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
    public class JSTests : BaseTest
    {
        // Константа с ожидаемым текстом ошибки для невалидного ввода.
        private const string ExpecctedSuccessMessage = "Whoo Hoooo! Correct!";
        // Поле для хранения тестового пользователя в рамках одного теста.
        private UserModel user;

        // SetUp-метод выполняется перед каждым тестом в этом классе.
        [SetUp]

        public void JSTestSetUp()
        {

            user = UserModel.GetDefaultUser();
            SiteNavigator.NavigateToLoginPage(Driver).Login(user);

        }

        [Test] //Это NUnit-атрибут, который помечает метод как тестовый кейс для выполнения тест-раннером.

        public void JS_Test_Page_Should_Process_Correct_Coordinates_From_JavaScript()
        {
            // Arrange: Открываем страницу JS и ищем координаты.
            var jsTestPage = SiteNavigator.NavigateToJSPage(Driver);

            // Act: Получаем координаты элемента на странице, используя JavaScript.
            var coordinates = jsTestPage.GetJumpingCoordinates();

            jsTestPage.EnterTopCoordinate(coordinates[0]);
            jsTestPage.EnterLeftCoordinate(coordinates[1]);
            jsTestPage.ClickProcessButton();

            var alertText = jsTestPage.GetAndAcceptAlertText();

            // Assert: Проверяем, что текст алерта соответствует ожидаемому результату.
            Assert.That(alertText, Is.EqualTo(ExpecctedSuccessMessage), "The alert text does not match the expected success message.");

        }

    }
}
