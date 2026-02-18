using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using OpenQA.Selenium;
using Selenium.Framework;
using Selenium.Framework.Models;
using Selenium.Pages;
using static Selenium.Pages.BasePage;

namespace Selenium.Tests
{
    public class RegistrationTests : BaseTest
    {
        [Test]
        public void Register_New_User_And_Verify_Logged_In()
        {
            var user = UserModel.GetRandom();
            user.ConfirmPassword = user.Password;

            // Навигация на страницу регистрации через SiteNavigator
            var registrationPage = SiteNavigator.NavigateToRegisterPage(Driver);

            BasePage.WaitHelper.WaitForElementVisible(Driver, By.Name("name"));
            registrationPage.Register(user, RegistrationPage.RoleType.User);    //в последнем выражении RoleType. выбираем нужный тип роли

            BasePage.WaitHelper.WaitForElementVisible(Driver, By.CssSelector(".welcome"));
            var header = new Header(Driver);
            var welcome = header.GetWelcomeText;

            Assert.That(welcome, Is.Not.Null.And.Not.Empty);
            Assert.That(welcome, Does.Contain(user.Login).Or.Contain(user.FirstName));

        }

        [Test]
        public void Register_Logout_And_Verify_Can_Login_Again()
        {
            // Arrange: создаём нового пользователя
            var user = UserModel.GetRandom();
            user.ConfirmPassword = user.Password;

            var registrationPage = SiteNavigator.NavigateToRegisterPage(Driver);
            BasePage.WaitHelper.WaitForElementVisible(Driver, By.Name("name"));
            registrationPage.Register(user, RegistrationPage.RoleType.User);

            // Проверяем что после регистрации мы залогинены
            BasePage.WaitHelper.WaitForElementVisible(Driver, By.CssSelector(".welcome"));
            var header = new Header(Driver);
            Assert.That(header.GetWelcomeText, Does.Contain(user.Login).Or.Contain(user.FirstName));

            // Act: logout
            var loginPageAfterLogout = header.Logout();
            BasePage.WaitHelper.WaitForElementVisible(Driver, By.Id("j_username")); // поле логина

            // Act: логинимся вновь тем же пользователем
            var homePage = loginPageAfterLogout.Login(user);

            // Assert: снова видим приветствие с тем же пользователем
            BasePage.WaitHelper.WaitForElementVisible(Driver, By.CssSelector(".welcome"));
            var welcomeAgain = homePage.OnHeader().GetWelcomeText;
            Assert.That(welcomeAgain, Does.Contain(user.Login).Or.Contain(user.FirstName));
        }

        [Test]
        public void Register_Developer_And_Verify_Can_Upload_Application()
        {
            var user = UserModel.GetRandom();
            user.ConfirmPassword = user.Password;

            var registrationPage = SiteNavigator.NavigateToRegisterPage(Driver);

            BasePage.WaitHelper.WaitForElementVisible(Driver, By.Name("name"));
            registrationPage.Register(user, RegistrationPage.RoleType.Developer);

            // Verify developer is logged in
            BasePage.WaitHelper.WaitForElementVisible(Driver, By.CssSelector(".welcome"));
            var appsPage = new ApplicationsPage(Driver);
            Assert.That(appsPage.WelcomeLabel.Text, Does.Contain(user.Login).Or.Contain(user.FirstName));

            // Developer should see link 'My applications'
            Assert.That(appsPage.MyApplicationsLink, Is.Not.Null);
            appsPage.MyApplicationsLink.Click();

            // Ожидаем переход на страницу моих приложений (/my)
            BasePage.WaitHelper.WaitUntil(Driver, d => d.Url.Contains("/my"));
            var myAppsPage = new MyApplicationsPage(Driver);

            // Должна быть ссылка на создание нового приложения
            Assert.That(myAppsPage.AddNewApplicationLink.Displayed);
            myAppsPage.AddNewApplicationLink.Click();

            // Ожидаем загрузку страницы создания приложения
            BasePage.WaitHelper.WaitForElementVisible(Driver, By.Name("title"));
            var newAppPage = new NewApplicationPage(Driver);

            // Assertions: наличие ключевых элементов формы
            Assert.That(newAppPage.TitleInput.Displayed);
            Assert.That(newAppPage.DescriptionTextArea.Displayed);
            Assert.That(newAppPage.CategorySelect.Displayed);
            Assert.That(newAppPage.ImageUpload.Displayed);
            Assert.That(newAppPage.IconUpload.Displayed);
            Assert.That(newAppPage.CreateButton.Displayed);

        }

        [Test]
        public void Register_RegUser_And_Verify_Cant_Upload_Application()
        {
            var user = UserModel.GetRandom();
            user.ConfirmPassword = user.Password;

            var registrationPage = SiteNavigator.NavigateToRegisterPage(Driver);

            BasePage.WaitHelper.WaitForElementVisible(Driver, By.Name("name"));
            registrationPage.Register(user, RegistrationPage.RoleType.User);

            // Verify RegUser is logged in
            BasePage.WaitHelper.WaitForElementVisible(Driver, By.CssSelector(".welcome"));
            var appsPage = new ApplicationsPage(Driver);
            Assert.That(appsPage.WelcomeLabel.Text, Does.Contain(user.Login).Or.Contain(user.FirstName));

            // RegUser should not see link 'My applications'
            Assert.That(appsPage.MyApplicationsLink, Is.Null);
        }

        [Test]
        public void Register_5_Users_From_CSV()
        {
            string testUserFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Tests", "TestData", "Users.csv");
            bool testUsetFileExists = File.Exists(testUserFilePath);
            string[] userLines = File.ReadAllLines(testUserFilePath); // читаем все строки из файла

            List<UserModelExtended> userList = new List<UserModelExtended>(userLines.Length - 1);
            for (int i = 1; i < userLines.Length; i++) // пропускаем заголовок
            {
                string line = userLines[i];
                string[] parts = line.Split(new char[] { ';' }, System.StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 4)
                {
                    var user = new UserModelExtended
                    {
                        Login = parts[0].Trim(),
                        FirstName = parts[1].Trim(),
                        LastName = parts[2].Trim(),
                        Password = parts[3].Trim(),
                        Role = parts.Length > 4 && string.IsNullOrEmpty(parts[4]) ? parts[4].Trim() : "USER"
                    };
                    user.ConfirmPassword = user.Password;
                    userList.Add(user);
                }
            }

            Assert.That(testUsetFileExists, Is.True, $"Test data file not found: {testUserFilePath}");
            Assert.That(userLines.Length, Is.GreaterThan(0), "Invalid or Empty file.");
            Assert.That(userList.Count, Is.EqualTo(userLines.Length - 1), $"Expected {userLines.Length - 1} users in test data.");
        }
    }
}
