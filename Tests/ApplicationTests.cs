// Подключаем System для базовых типов (Guid, StringComparison и т.д.).
// Подключаем NUnit-атрибуты и Assert.
using NUnit.Framework;
// Подключаем Selenium By и IWebElement.
using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V142.Storage;
// Подключаем SiteNavigator и прочую инфраструктуру.
using Selenium.Framework;
// Подключаем модели пользователей.
using Selenium.Framework.Models;
// Подключаем Page Object'ы.
using Selenium.Pages;
using System;
// Подключаем коллекции (IEnumerable, List и т.п.).
using System.Collections.Generic;
// Подключаем работу с файлами (File, Path).
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Web.Security.AntiXss;
// Подключаем WaitHelper статически для компактного синтаксиса.
using static Selenium.Pages.BasePage;
using static System.Net.Mime.MediaTypeNames;

// Пространство имён набора тестов.
namespace Selenium.Tests
{
    public class ApplicationTests : BaseTest
    {
        // Поле для хранения тестового пользователя в рамках одного теста.
        private UserModel user;

        [SetUp]
        protected void Initialize() //перед каждым тестом, как признак “логин успешен”
        {
            user= UserModel.GetDefaultUser();
            SiteNavigator.NavigateToLoginPage(Driver).Login(user);
            WaitHelper.WaitForElementVisible(Driver, By.CssSelector(".welcome"));
        }
        [Test]
        public void Download_Should_Open_Json_With_Data_Equal_To_Details_Page()
        {
            OpenFirstAppDetails();

            var card = new ApplicationCard(Driver);
            var uiTitle = card.GetAppName();
            var uiDescription = GetTextAfterColon(card.GetDescription());
            var uiCategory = GetTextAfterColon(card.GetCategory());
            var uiAuthor = GetTextAfterColon(card.GetAuthor());
            var uiDownloads = ParseDownloads(card.GetDownloads());

            card.Download();

            var json = GetJsonFromCurrentPage();
            var jsonTitle = GetString(json,"title");
            var jsonDescribtion = GetString(json, "description");
            var jsonCategory = GetString(json, "category", "title");
            var jsonAuthor = GetString(json, "author", "name");
            var jsonDownloads = GetInt(json, "numberOfDownloads");

            Assert.That(jsonTitle, Is.EqualTo(uiTitle));
            Assert.That(jsonDescribtion, Is.EqualTo(uiDescription));
            Assert.That(jsonCategory, Is.EqualTo(uiCategory));
            Assert.That(jsonAuthor, Is.EqualTo(uiAuthor));
            Assert.That(jsonDownloads, Is.EqualTo(uiDownloads+1));

        }

        [Test]
        public void Create_New_App_Without_Images()
        //Create a new application without images. Verify that it is displayed correctly and can be downloaded.
        {  
            var title = "New Test App " + Guid.NewGuid();
            var description = "New app without images";
            var category = "Information";

            var myApplicationsPage = SiteNavigator.NavigateToMyApplicationsPage(Driver);
            
            var newApplicationPage = myApplicationsPage.OpenNewApplicationForm();

            newApplicationPage.EnterTitle(title);
            newApplicationPage.EnterDescription(description);
            newApplicationPage.SelectCategory(category);
            
            newApplicationPage.ClickCreateButton();

            // Повторно создаём объект страницы — EnsureLoaded выполнится
            var myAppAfterCreate = new MyApplicationsPage(Driver);
            // Ищем карточку
            var createdAppCard = myAppAfterCreate.FindAppCardByName(title);
            Assert.That(createdAppCard, Is.Not.Null);
            // Открываем детали
            myAppAfterCreate.OpenAppDetailsByName(title);

            var card = new ApplicationCard(Driver);
            Assert.That(card.GetAppName(), Is.EqualTo(title));
            Assert.That(GetTextAfterColon(card.GetDescription()), Is.EqualTo(description));
            Assert.That(GetTextAfterColon(card.GetCategory()), Is.EqualTo(category));

            // Проверяем JSON
            card.Download();

            var json = GetJsonFromCurrentPage();

            Assert.That(GetString(json, "title"), Is.EqualTo(title));
            Assert.That(GetString(json, "description"), Is.EqualTo(description));
            Assert.That(GetString(json, "category", "title"), Is.EqualTo(category));
            Assert.That(json.GetProperty("imageData").ValueKind, Is.EqualTo(JsonValueKind.Null)); //проверяем, что в JSON нет данных об изображении, так как мы не добавляли изображения при создании приложения
            Assert.That(json.GetProperty("iconData").ValueKind, Is.EqualTo(JsonValueKind.Null)); //проверяем, что в JSON нет данных об иконке, так как мы не добавляли иконку при создании приложения

        }

        [Test]
        public void Create_New_App_With_Images()
        {
            var title = "New Test App" + Guid.NewGuid();
            var description = "New app with images";
            var category = "Information";

            var imagePath = GetTestDataPath("Image jpeg.jpg");
            var iconPath = GetTestDataPath("icon image.jpg");

            //var imagePath = "C:\\Users\\npopova\\Downloads\\SeleniumProject\\Selenium\\Tests\\TestData\\Image jpeg.jpg";
            //var iconPath = "C:\\Users\npopova\\Downloads\\SeleniumProject\\Selenium\\Tests\\TestData\\icon image.jpg";

            var myApplicationPage = SiteNavigator.NavigateToMyApplicationsPage(Driver);
            var newApplicationPage = myApplicationPage.OpenNewApplicationForm();

            newApplicationPage.FillNewApplicationForm(title, description, category, imagePath, iconPath);

            newApplicationPage.ClickCreateButton();

            var myAppAfterCreate = new MyApplicationsPage(Driver);
            var createAppCard = myAppAfterCreate.FindAppCardByName(title);
            Assert.That(createAppCard, Is.Not.Null);

            myAppAfterCreate.OpenAppDetailsByName(title);

            var card = new ApplicationCard(Driver);
            Assert.That(card.GetAppName, Is.EqualTo(title));
            Assert.That(card.GetDescription, Is.EqualTo(description));
            Assert.That(card.GetCategory, Is.EqualTo(category));

        }

        private string GetTestDataPath(string fileName)
        {
            var filePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Tests", "TestData", fileName);
            Assert.That(File.Exists(filePath), Is.True, $"Test data file not found: {filePath}");
            return filePath;
        }


        [Test]
        //Edit an application without images, and verify that the changes were applied.
        public void Edite_App_Without_Images()
        {
            var title = "New Test App " + Guid.NewGuid();
            var description = "New app without images";
            var category = "Information";

            var myApplicationsPage = SiteNavigator.NavigateToMyApplicationsPage(Driver);

            var newApplicationPage = myApplicationsPage.OpenNewApplicationForm();

            newApplicationPage.EnterTitle(title);
            newApplicationPage.EnterDescription(description);
            newApplicationPage.SelectCategory(category);

            newApplicationPage.ClickCreateButton();

            // Повторно создаём объект страницы — EnsureLoaded выполнится
            var myAppAfterCreate = new MyApplicationsPage(Driver);
            // Ищем карточку
            var createdAppCard = myAppAfterCreate.FindAppCardByName(title);
            Assert.That(createdAppCard, Is.Not.Null);
            // Открываем детали
            myAppAfterCreate.OpenAppDetailsByName(title);

            var card = new ApplicationCard(Driver);

            card.Click_Edit_Button();


            Assert.That(card.GetAppName(), Is.EqualTo(title));
            Assert.That(GetTextAfterColon(card.GetDescription()), Is.EqualTo(description));
            Assert.That(GetTextAfterColon(card.GetCategory()), Is.EqualTo(category));

        }

        private int GetInt(JsonElement root, string prop)
        {
            return root.GetProperty(prop).GetInt32();
        }

        private static string GetString(JsonElement root, string prop1, string prop2 = null)
        //prop1 - это имя свойства верхнего уровня
        //prop2 - это имя вложенного свойства, которое нужно получить, если оно указано.
        //Если prop2 не указано, то возвращаем значение prop1.
        {
            var element = root.GetProperty(prop1);
            if (prop2 != null)
            {
                element = element.GetProperty(prop2);
            }
            return element.GetString();
        }

        private JsonElement GetJsonFromCurrentPage()
        {
            // Ждём JSON внутри <pre> и берём его текст
            var pre = WaitHelper.WaitForElementVisible(Driver, By.TagName("pre")); // это HTML-элемент <pre>
            var jsonText = pre.Text; // это текст внутри этого элемента, который должен быть JSON-строкой

            using (var doc = JsonDocument.Parse(jsonText))
            {
              return doc.RootElement.Clone();
            }

        }

        private static int ParseDownloads(string text)
        {
            var digits = new string(text.Where(char.IsDigit).ToArray()); //оставляет только цифры и превращает их в массив символов и собирает обратно в строку
            if (string.IsNullOrEmpty(digits))
            {
              return 0;  
            }
            return int.Parse(digits); // превращает строку в число, если строка не является числом, то выбрасывает исключение. Поэтому сначала проверяем, что строка не пустая и содержит цифры.
        }

        private static string GetTextAfterColon(string text) // static - не использует поля класса, работает только с парам text
        {
           var parts = text.Split(new[] { ':' }, 2); //Если нужно ограничить количество частей при Split: всегда передаём массив символов
            if (parts.Length == 2) 
            {
                return parts[1].Trim();
            }
            return text.Trim();
        }

        private void OpenFirstAppDetails()
        {
            var link = WaitHelper.WaitForElementClickable(Driver, By.CssSelector("a[href='/app?title=Application Information 0']"));
            link.Click();
            WaitHelper.WaitForElementVisible(Driver, By.CssSelector("div.name"));
        }

        private void OpenAllAppsPage()
        {
            Driver.Navigate().GoToUrl(Settings.GetBaseUrl().TrimEnd('/')+ "/all");
            WaitHelper.WaitForElementVisible(Driver, By.CssSelector(".apps"));
        }
    }

}
