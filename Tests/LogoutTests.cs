// Подключаем NUnit для атрибутов тестов и проверок.
using NUnit.Framework;
// Подключаем Selenium-тип `By`, `WindowType` и т.п.
using OpenQA.Selenium;
// Подключаем навигацию по страницам.
using Selenium.Framework;
// Подключаем модель пользователя.
using Selenium.Framework.Models;
// Подключаем Page Object классы.
using Selenium.Pages;
// Подключаем статические методы ожиданий (WaitHelper).
using static Selenium.Pages.BasePage;

// Пространство имён для тестов.
namespace Selenium.Tests
{
    // Класс тестов logout, наследуется от BaseTest.
    public class LogoutTests : BaseTest
    {
        // Поле с пользователем, который будет логиниться в тесте.
        private UserModel user;

        // Выполняется перед каждым тестом.
        [SetUp]
        protected void Initialize()
        {
            // Загружаем валидного пользователя.
            user = UserModel.GetDefaultUser();
        }

        // Тест из задания: logout во второй вкладке должен инвалидировать сессию в первой.
        [Test]
        public void Logout_In_New_Tab_Invalidates_First_Tab_Session()
        {
            // Логинимся на первой вкладке.
            var homePage = SiteNavigator.NavigateToLoginPage(Driver).Login(user);
            // Ждём подтверждение успешного логина по приветствию в хедере.
            WaitHelper.WaitUntil(Driver, d => homePage.OnHeader().GetWelcomeText.Contains(user.FirstName));

            // Сохраняем идентификатор первой вкладки, чтобы вернуться к ней позже.
            var firstTab = Driver.CurrentWindowHandle;

            // Открываем новую вкладку браузера и автоматически переключаемся в неё.
            Driver.SwitchTo().NewWindow(WindowType.Tab);
            // На второй вкладке идём на Home и выполняем logout.
            SiteNavigator.NavigateToHomePage(Driver).Logout(user);
            // Ждём переход на страницу логина после выхода.
            WaitHelper.WaitUntil(Driver, d => d.Url.Contains("/login"));

            // Закрываем вторую вкладку (где был logout).
            Driver.Close();
            // Возвращаемся на первую вкладку по сохранённому handle.
            Driver.SwitchTo().Window(firstTab);

            // Ищем кликабельную ссылку All на первой вкладке.
            var allLink = WaitHelper.WaitForElementClickable(Driver, By.LinkText("All"));
            // Нажимаем ссылку, провоцируя действие в уже инвалидированной сессии.
            allLink.Click();

            // Проверяем, что нас редиректит на логин (сессия сброшена глобально).
            WaitHelper.WaitUntil(Driver, d => d.Url.Contains("/login"), timeoutSeconds: 10);
            // Финальная проверка URL.
            Assert.That(Driver.Url, Does.Contain("/login"));
        }
    }
}
