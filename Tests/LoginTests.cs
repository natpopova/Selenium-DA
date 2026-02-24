// Подключаем базовые атрибуты и Assert из NUnit.
using NUnit.Framework;
// Подключаем legacy-ассерты (ClassicAssert.True и др.).
using NUnit.Framework.Legacy;
// Подключаем типы Selenium (например, IWebDriver, By).
using OpenQA.Selenium;
// Подключаем навигатор по страницам сайта и инфраструктуру тестов.
using Selenium.Framework;
// Подключаем модель пользователя, которую используем как тестовые данные.
using Selenium.Framework.Models;
// Подключаем Page Object классы.
using Selenium.Pages;
// Импортируем статический класс WaitHelper для удобного вызова ожиданий.
using static Selenium.Pages.BasePage;



// Пространство имён для UI-тестов проекта.
namespace Selenium.Tests
{
    // Класс тестов логина наследуется от BaseTest (там инициализация/закрытие драйвера).
    public class LoginTests : BaseTest
    {
        // Константа с ожидаемым текстом ошибки для невалидного входа.
        private const string InvalidLoginMessage = "invalid username or password";
        // Поле для хранения тестового пользователя в рамках одного теста.
        private UserModel user;

        // SetUp-метод выполняется перед каждым тестом в этом классе.
        [SetUp]
        protected void Initialize()
        {
            // Берём дефолтного валидного пользователя (admin/admin).
            //Вызываем статический метод GetDefaultUser() класса UserModel, который создаёт и возвращает экземпляр пользователя с дефолтными данными.
            user = UserModel.GetDefaultUser();
        }

        // Тест: вход валидным пользователем.
        [Test] //Это NUnit-атрибут, который помечает метод как тестовый кейс для выполнения тест-раннером.
        
        public void Login_As_Valid_User() //Публичный метод без возвращаемого значения, представляющий тест-сценарий “логин валидным пользователем”.
        {
            // Переходим на страницу логина и выполняем вход, получая объект домашней страницы.
            var homePage = SiteNavigator.NavigateToLoginPage(Driver).Login(user);

            // Пишем инф сообщение в лог.
            Logger.Info("Assert user login");
            // Ждём, пока в приветствии появится имя пользователя. вспомогательный метод
            WaitHelper.WaitUntil(Driver, d => homePage.OnHeader().GetWelcomeText.Contains(user.FirstName));
            // Проверяем, что приветствие действительно содержит имя.
            Assert.That(homePage.OnHeader().GetWelcomeText, Does.Contain(user.FirstName));
        }

        // Тест: валидный логин + неправильный пароль.
        [Test]
        public void Login_As_Valid_User_With_Incorrect_Password()
        {
            // Переопределяем пароль на заведомо неправильный.
            user.Password = "invalid";

            // Переходим на страницу логина.
            var loginPage = SiteNavigator.NavigateToLoginPage(Driver);
            // Пытаемся войти с неверным паролем.
            loginPage.Login(user);

            // Ждём появления flash-сообщения об ошибке авторизации.
            WaitHelper.WaitUntil(Driver, d => loginPage.GetFlashMessage().Contains(InvalidLoginMessage));
            // Проверяем текст сообщения об ошибке.
            Assert.That(loginPage.GetFlashMessage(), Does.Contain(InvalidLoginMessage), "invalid username or password");
            // Проверяем, что мы остались на странице логина.
            Assert.That(Driver.Url, Does.Contain("/login"));
        }
    }
}
