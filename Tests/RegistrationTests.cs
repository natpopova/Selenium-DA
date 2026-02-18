// Подключаем System для базовых типов (Guid, StringComparison и т.д.).
using System;
// Подключаем коллекции (IEnumerable, List и т.п.).
using System.Collections.Generic;
// Подключаем работу с файлами (File, Path).
using System.IO;
// Подключаем NUnit-атрибуты и Assert.
using NUnit.Framework;
// Подключаем Selenium By и IWebElement.
using OpenQA.Selenium;
// Подключаем SiteNavigator и прочую инфраструктуру.
using Selenium.Framework;
// Подключаем модели пользователей.
using Selenium.Framework.Models;
// Подключаем Page Object'ы.
using Selenium.Pages;
// Подключаем WaitHelper статически для компактного синтаксиса.
using static Selenium.Pages.BasePage;

// Пространство имён набора тестов.
namespace Selenium.Tests
{
    // Класс тестов регистрации и прав доступа, наследуется от BaseTest.
    public class RegistrationTests : BaseTest
    {
        // Локатор приветствия в хедере (используем в проверках успешного логина).
        private static readonly By WelcomeSelector = By.CssSelector(".welcome");

        // Сценарий: зарегистрировать нового пользователя и проверить, что он авторизован.
        [Test]
        public void Register_New_User_And_Verify_Logged_In()
        {
            // Генерируем случайного пользователя для изоляции теста.
            var user = BuildRandomUser();

            // Переходим на регистрацию и отправляем форму с ролью обычного пользователя.
            SiteNavigator.NavigateToRegisterPage(Driver)
                .Register(user, RegistrationPage.RoleType.User);

            // Проверяем факт авторизации после регистрации.
            AssertUserIsLoggedIn(user);
        }

        // Сценарий: регистрация -> logout -> повторный login тем же пользователем.
        [Test]
        public void Register_Logout_And_Verify_Can_Login_Again()
        {
            // Генерируем пользователя.
            var user = BuildRandomUser();

            // Регистрируем нового пользователя.
            SiteNavigator.NavigateToRegisterPage(Driver)
                .Register(user, RegistrationPage.RoleType.User);

            // Валидируем, что сразу после регистрации пользователь залогинен.
            var header = AssertUserIsLoggedIn(user);
            // Выходим из аккаунта и получаем страницу логина.
            var loginPage = header.Logout();

            // Ждём поле логина как признак отображения страницы login.
            WaitHelper.WaitForElementVisible(Driver, By.Id("j_username"));
            // Снова логинимся тем же пользователем.
            var homePage = loginPage.Login(user);

            // Ждём приветствие после повторного входа.
            WaitHelper.WaitForElementVisible(Driver, WelcomeSelector);
            // Проверяем, что приветствие соответствует этому пользователю.
            Assert.That(homePage.OnHeader().GetWelcomeText, Does.Contain(user.Login).Or.Contain(user.FirstName));
        }

        // Сценарий: регистрация разработчика и проверка доступа к странице загрузки приложения.
        [Test]
        public void Register_Developer_And_Verify_Can_Open_Upload_Page()
        {
            // Создаём случайного пользователя.
            var user = BuildRandomUser();

            // Регистрируем его как Developer.
            SiteNavigator.NavigateToRegisterPage(Driver)
                .Register(user, RegistrationPage.RoleType.Developer);

            // Подтверждаем, что после регистрации пользователь авторизован.
            AssertUserIsLoggedIn(user);

            // Создаём объект страницы каталога приложений.
            var appsPage = new ApplicationsPage(Driver);
            // Для Developer ссылка "My applications" должна существовать.
            Assert.That(appsPage.MyApplicationsLink, Is.Not.Null, "Developer should see 'My applications' link.");

            // Переходим в раздел "My applications".
            appsPage.MyApplicationsLink.Click();
            // Ждём перехода по URL.
            WaitHelper.WaitUntil(Driver, d => d.Url.Contains("/my"));

            // Инициализируем страницу "My applications".
            var myAppsPage = new MyApplicationsPage(Driver);
            // Проверяем наличие ссылки создания/загрузки нового приложения.
            Assert.That(myAppsPage.AddNewApplicationLink.Displayed, "Developer should see upload/new app link.");

            // Переходим на страницу создания приложения.
            myAppsPage.AddNewApplicationLink.Click();
            // Ждём URL новой страницы.
            WaitHelper.WaitUntil(Driver, d => d.Url.Contains("/new"));

            // Инициализируем страницу создания приложения.
            var newAppPage = new NewApplicationPage(Driver);
            // Проверяем, что форма доступна (есть кнопка Create).
            Assert.That(newAppPage.CreateButton.Displayed, "Upload form should be available for developer.");
        }

        // Сценарий: обычный пользователь видит приложения, но не может загружать новые.
        [Test]
        public void Register_Regular_User_And_Verify_Can_See_But_Cant_Upload_Application()
        {
            // Генерируем случайного пользователя.
            var user = BuildRandomUser();

            // Регистрируем как обычного пользователя.
            SiteNavigator.NavigateToRegisterPage(Driver)
                .Register(user, RegistrationPage.RoleType.User);

            // Проверяем авторизацию после регистрации.
            AssertUserIsLoggedIn(user);

            // Открываем страницу приложений.
            var appsPage = new ApplicationsPage(Driver);
            // Проверяем, что карточки приложений доступны для просмотра.
            Assert.That(appsPage.AppCards.Count, Is.GreaterThan(0), "Regular user should see application cards.");
            // Проверяем отсутствие ссылки на загрузку/управление своими приложениями.
            Assert.That(appsPage.MyApplicationsLink, Is.Null, "Regular user should not have upload entry point.");
        }

        // DDT: источник тест-кейсов — CSV; один прогон на одну запись файла.
        [TestCaseSource(nameof(RegistrationUsersFromCsv))]
        public void Register_Users_From_Csv_Ddt(UserModelExtended csvUser)
        {
            // Преобразуем роль из CSV в enum, который понимает RegistrationPage.
            var role = ParseRole(csvUser.Role);
            // Создаём копию данных пользователя и делаем логин уникальным для повторяемости прогона.
            var user = new UserModelExtended
            {
                // Добавляем случайный суффикс, чтобы избежать конфликта "user already exists".
                Login = $"{csvUser.Login}_{Guid.NewGuid():N}".Substring(0, Math.Min(30, csvUser.Login.Length + 9)),
                // Копируем имя.
                FirstName = csvUser.FirstName,
                // Копируем фамилию.
                LastName = csvUser.LastName,
                // Копируем пароль.
                Password = csvUser.Password,
                // Подтверждение пароля совпадает с паролем.
                ConfirmPassword = csvUser.Password,
                // Сохраняем роль для читаемости и отладки.
                Role = csvUser.Role
            };

            // Регистрируем пользователя с ролью из CSV.
            SiteNavigator.NavigateToRegisterPage(Driver)
                .Register(user, role);

            // Проверяем, что регистрация завершилась логином.
            AssertUserIsLoggedIn(user);

            // Открываем приложения для role-based проверок прав.
            var appsPage = new ApplicationsPage(Driver);
            // Если это developer — ссылка "My applications" обязана быть.
            if (role == RegistrationPage.RoleType.Developer)
            {
                Assert.That(appsPage.MyApplicationsLink, Is.Not.Null, "Developer from CSV must be able to upload.");
            }
            // Если обычный пользователь — ссылки быть не должно.
            else
            {
                Assert.That(appsPage.MyApplicationsLink, Is.Null, "Regular user from CSV must not be able to upload.");
            }
        }

        // Метод-генератор данных для TestCaseSource (читает пользователей из CSV).
        private static IEnumerable<UserModelExtended> RegistrationUsersFromCsv()
        {
            // Формируем путь до CSV в выходной директории тестов.
            var csvPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Tests", "TestData", "users.csv");
            // Проверяем, что файл действительно присутствует.
            Assert.That(File.Exists(csvPath), Is.True, $"Test data file not found: {csvPath}");

            // Читаем все строки файла.
            var lines = File.ReadAllLines(csvPath);
            // Проверяем минимум: заголовок + 5 пользователей (по заданию).
            Assert.That(lines.Length, Is.GreaterThanOrEqualTo(6), "CSV should contain header + at least 5 users.");

            // Начинаем с 1, потому что нулевая строка — это header.
            for (var i = 1; i < lines.Length; i++)
            {
                // Берём текущую строку и убираем лишние пробелы по краям.
                var line = lines[i].Trim();
                // Пропускаем пустые строки.
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                // Делим строку по разделителю ';'.
                var parts = line.Split(';');
                // Проверяем корректность количества колонок.
                if (parts.Length < 5)
                {
                    // Выбрасываем исключение с номером проблемной строки.
                    throw new FormatException($"CSV line {i + 1} must contain 5 columns: {line}");
                }

                // Возвращаем объект пользователя как один тестовый кейс.
                yield return new UserModelExtended
                {
                    // Колонка логина.
                    Login = parts[0].Trim(),
                    // Колонка имени.
                    FirstName = parts[1].Trim(),
                    // Колонка фамилии.
                    LastName = parts[2].Trim(),
                    // Колонка пароля.
                    Password = parts[3].Trim(),
                    // ConfirmPassword дублирует пароль.
                    ConfirmPassword = parts[3].Trim(),
                    // Колонка роли.
                    Role = parts[4].Trim()
                };
            }
        }

        // Вспомогательный метод: перевод строковой роли в enum роли страницы регистрации.
        private static RegistrationPage.RoleType ParseRole(string role)
        {
            // Если роль "DEVELOPER" (без учёта регистра), возвращаем Developer.
            return string.Equals(role, "DEVELOPER", StringComparison.OrdinalIgnoreCase)
                ? RegistrationPage.RoleType.Developer
                // Иначе по умолчанию считаем роль обычным пользователем.
                : RegistrationPage.RoleType.User;
        }

        // Вспомогательный фабричный метод для генерации валидного random user.
        private static UserModel BuildRandomUser()
        {
            // Получаем случайного пользователя из модели.
            var user = UserModel.GetRandom();
            // Подтверждение пароля делаем равным паролю.
            user.ConfirmPassword = user.Password;
            // Возвращаем готовый объект.
            return user;
        }

        // Вспомогательная проверка: пользователь залогинен (по блоку приветствия).
        private Header AssertUserIsLoggedIn(UserModel user)
        {
            // Ждём видимость приветствия.
            WaitHelper.WaitForElementVisible(Driver, WelcomeSelector);
            // Создаём объект Header для чтения приветственного текста и действий.
            var header = new Header(Driver);

            // Проверяем, что приветствие относится к нужному пользователю.
            Assert.That(header.GetWelcomeText, Does.Contain(user.Login).Or.Contain(user.FirstName));
            // Возвращаем header для дальнейшего использования вызывающей стороной.
            return header;
        }
    }
}
