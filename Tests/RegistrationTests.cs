using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using OpenQA.Selenium;
using Selenium.Framework;
using Selenium.Framework.Models;
using Selenium.Pages;
using static Selenium.Pages.BasePage;

namespace Selenium.Tests
{
    public class RegistrationTests : BaseTest
    {
        private static readonly By WelcomeSelector = By.CssSelector(".welcome");

        [Test]
        public void Register_New_User_And_Verify_Logged_In()
        {
            var user = BuildRandomUser();

            SiteNavigator.NavigateToRegisterPage(Driver)
                .Register(user, RegistrationPage.RoleType.User);

            AssertUserIsLoggedIn(user);
        }

        [Test]
        public void Register_Logout_And_Verify_Can_Login_Again()
        {
            var user = BuildRandomUser();

            SiteNavigator.NavigateToRegisterPage(Driver)
                .Register(user, RegistrationPage.RoleType.User);

            var header = AssertUserIsLoggedIn(user);
            var loginPage = header.Logout();

            WaitHelper.WaitForElementVisible(Driver, By.Id("j_username"));
            var homePage = loginPage.Login(user);

            WaitHelper.WaitForElementVisible(Driver, WelcomeSelector);
            Assert.That(homePage.OnHeader().GetWelcomeText, Does.Contain(user.Login).Or.Contain(user.FirstName));
        }

        [Test]
        public void Register_Developer_And_Verify_Can_Open_Upload_Page()
        {
            var user = BuildRandomUser();

            SiteNavigator.NavigateToRegisterPage(Driver)
                .Register(user, RegistrationPage.RoleType.Developer);

            AssertUserIsLoggedIn(user);

            var appsPage = new ApplicationsPage(Driver);
            Assert.That(appsPage.MyApplicationsLink, Is.Not.Null, "Developer should see 'My applications' link.");

            appsPage.MyApplicationsLink.Click();
            WaitHelper.WaitUntil(Driver, d => d.Url.Contains("/my"));

            var myAppsPage = new MyApplicationsPage(Driver);
            Assert.That(myAppsPage.AddNewApplicationLink.Displayed, "Developer should see upload/new app link.");

            myAppsPage.AddNewApplicationLink.Click();
            WaitHelper.WaitUntil(Driver, d => d.Url.Contains("/new"));

            var newAppPage = new NewApplicationPage(Driver);
            Assert.That(newAppPage.CreateButton.Displayed, "Upload form should be available for developer.");
        }

        [Test]
        public void Register_Regular_User_And_Verify_Can_See_But_Cant_Upload_Application()
        {
            var user = BuildRandomUser();

            SiteNavigator.NavigateToRegisterPage(Driver)
                .Register(user, RegistrationPage.RoleType.User);

            AssertUserIsLoggedIn(user);

            var appsPage = new ApplicationsPage(Driver);
            Assert.That(appsPage.AppCards.Count, Is.GreaterThan(0), "Regular user should see application cards.");
            Assert.That(appsPage.MyApplicationsLink, Is.Null, "Regular user should not have upload entry point.");
        }

        [TestCaseSource(nameof(RegistrationUsersFromCsv))]
        public void Register_Users_From_Csv_Ddt(UserModelExtended csvUser)
        {
            var role = ParseRole(csvUser.Role);
            var user = new UserModelExtended
            {
                Login = $"{csvUser.Login}_{Guid.NewGuid():N}".Substring(0, Math.Min(30, csvUser.Login.Length + 9)),
                FirstName = csvUser.FirstName,
                LastName = csvUser.LastName,
                Password = csvUser.Password,
                ConfirmPassword = csvUser.Password,
                Role = csvUser.Role
            };

            SiteNavigator.NavigateToRegisterPage(Driver)
                .Register(user, role);

            AssertUserIsLoggedIn(user);

            var appsPage = new ApplicationsPage(Driver);
            if (role == RegistrationPage.RoleType.Developer)
            {
                Assert.That(appsPage.MyApplicationsLink, Is.Not.Null, "Developer from CSV must be able to upload.");
            }
            else
            {
                Assert.That(appsPage.MyApplicationsLink, Is.Null, "Regular user from CSV must not be able to upload.");
            }
        }

        private static IEnumerable<UserModelExtended> RegistrationUsersFromCsv()
        {
            var csvPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Tests", "TestData", "users.csv");
            Assert.That(File.Exists(csvPath), Is.True, $"Test data file not found: {csvPath}");

            var lines = File.ReadAllLines(csvPath);
            Assert.That(lines.Length, Is.GreaterThanOrEqualTo(6), "CSV should contain header + at least 5 users.");

            for (var i = 1; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var parts = line.Split(';');
                if (parts.Length < 5)
                {
                    throw new FormatException($"CSV line {i + 1} must contain 5 columns: {line}");
                }

                yield return new UserModelExtended
                {
                    Login = parts[0].Trim(),
                    FirstName = parts[1].Trim(),
                    LastName = parts[2].Trim(),
                    Password = parts[3].Trim(),
                    ConfirmPassword = parts[3].Trim(),
                    Role = parts[4].Trim()
                };
            }
        }

        private static RegistrationPage.RoleType ParseRole(string role)
        {
            return string.Equals(role, "DEVELOPER", StringComparison.OrdinalIgnoreCase)
                ? RegistrationPage.RoleType.Developer
                : RegistrationPage.RoleType.User;
        }

        private static UserModel BuildRandomUser()
        {
            var user = UserModel.GetRandom();
            user.ConfirmPassword = user.Password;
            return user;
        }

        private Header AssertUserIsLoggedIn(UserModel user)
        {
            WaitHelper.WaitForElementVisible(Driver, WelcomeSelector);
            var header = new Header(Driver);

            Assert.That(header.GetWelcomeText, Does.Contain(user.Login).Or.Contain(user.FirstName));
            return header;
        }
    }
}
