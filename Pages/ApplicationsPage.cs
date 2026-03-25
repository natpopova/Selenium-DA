using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace Selenium.Pages
{
    public class ApplicationsPage : BasePage
    {
        // Локаторы основных блоков страницы.
        private readonly By _welcome = By.CssSelector(".welcome");
        private readonly By _popularApps = By.CssSelector(".popular-container .popular-app");
        private readonly By _popularAppName = By.CssSelector("popular-container .popular-app .name");
        private readonly By _popularAppDetailsLink = By.CssSelector(".popular-container .popular-app a[href*='app?title=']");
        private readonly By _categoryLinks = By.CssSelector(".categories-ul li a");
        private readonly By _appsContainer = By.CssSelector(".apps-container");
        private readonly By _appCards = By.CssSelector(".apps .app");

        // Локаторы внутренних элементов карточки приложения.
        private readonly By _appName = By.CssSelector(".name");
        private readonly By _appDescription = By.CssSelector(".description");
        private readonly By _appDownloads = By.CssSelector(".downloads");
        private readonly By _appDetailsLink = By.CssSelector("a[href*='app?title=']");

        // Локаторы ссылок меню.
        private readonly By _dumpLink = By.CssSelector(".footer a[href='/dump/']");
        private readonly By _myAppsLink = By.CssSelector("a[href='/my']");
        private readonly By _editAccountLink = By.CssSelector(".account a[href='/account']");
        private readonly By _logoutLink = By.CssSelector(".account a[href*='logout']");

        public ApplicationsPage(IWebDriver driver) : base(driver)
        {
            // Ждём главный контейнер, чтобы убедиться, что страница загрузилась.
            WaitHelper.WaitForElementVisible(Driver, _appsContainer);
        }

        // Веб-элементы как свойства. Это жёсткий поиск, который будет выбрасывать исключение, если элемент не найден. Это нормально для основных элементов страницы.
        public IWebElement WelcomeLabel => Driver.FindElement(_welcome);
        public IWebElement LogoutLink => Driver.FindElement(_logoutLink);
        public IWebElement EditAccountLink => Driver.FindElement(_editAccountLink);
        public IWebElement DumpLink => Driver.FindElement(_dumpLink);

        // Ссылка может отсутствовать для некоторых ролей, поэтому используем безопасный поиск.
        public IWebElement MyApplicationsLink => ElementIfExists(_myAppsLink);

        // Коллекции элементов на странице. Это мягкий поиск, который вернёт пустой список, если элементы не найдены. Это удобно для блоков, которые могут быть динамическими.
        public IReadOnlyCollection<IWebElement> PopularAppBlocks => Driver.FindElements(_popularApps);
        public IReadOnlyCollection<IWebElement> CategoryLinks => Driver.FindElements(_categoryLinks);
        public IReadOnlyCollection<IWebElement> AppCards => Driver.FindElements(_appCards);

        // Простая модель карточки для удобства в проверках.
        public class AppCard
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int Downloads { get; set; }
            public IWebElement DetailsLink { get; set; }
        }


        // Выполняем выход из аккаунта.
        public LoginPage Logout()
        {
            LogoutLink.Click();
            return new LoginPage(Driver);
        }

        // Открываем раздел "My applications".
        public MyApplicationsPage OpenMyApplications()
        {
            var link = MyApplicationsLink;

            if (link == null)
            {
                throw new NoSuchElementException("Ссылка 'My applications' отсутствует.");
            }

            link.Click();
            WaitHelper.WaitUntil(Driver, d => d.Url.Contains("/my"));
            return new MyApplicationsPage(Driver);
        }

        // Преобразуем текст формата "# of downloads: 10" в число 10.
        private int ParseDownloads(string text)
        {
            var parts = text.Split(':');

            if (parts.Length == 2)
            {
                int number;
                if (int.TryParse(parts[1].Trim(), out number))
                {
                    return number;
                }
            }

            return 0;
        }

        // Безопасно ищем элемент: если не найден, возвращаем null.
        private IWebElement ElementIfExists(By by)
        {
            var elements = Driver.FindElements(by);

            if (elements.Count > 0)
            {
                return elements[0];
            }

            return null;
        }
    }
}
