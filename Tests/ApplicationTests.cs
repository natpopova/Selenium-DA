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
            //OpenAllAppsPage();
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

        private JsonElement GetJsonFromCurrentPage() //если страница — это JSON,то PageSource уже равен JSON
        {
            //ждём, пока страница загрузится и её исходный код начнётся с {, что означает начало JSON-объекта
            WaitHelper.WaitUntil(Driver, d => d.PageSource.TrimStart().StartsWith("{"));

            // парсим весь PageSource как JSON
            //Dispose освобождает ресурсы
            //using вызывает Dispose автоматически
            //JsonDocument, Stream, File, SqlConnection — IDisposable

            using (var doc = JsonDocument.Parse(Driver.PageSource))
            {
                // возвращаем корневой элемент JSON, клонируя его, чтобы он не был связан с жизненным циклом документа
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
