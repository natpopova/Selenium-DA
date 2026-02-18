using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Pages
{
    /// <summary>
    /// Страница каталога приложений (Home / All / категории).
    /// </summary>
    public class ApplicationsPage : BasePage
    {
        // Локаторы
        private static readonly By _navigation = By.CssSelector(".navigation");
        private static readonly By _welcome = By.CssSelector(".welcome");
        private static readonly By _popularContainer = By.CssSelector(".popular-container");
        private static readonly By _popularApps = By.CssSelector(".popular-container .popular-app");
        private static readonly By _categoriesUl = By.CssSelector(".categories-ul");
        private static readonly By _categoryLinks = By.CssSelector(".categories-ul li a");
        private static readonly By _appsContainer = By.CssSelector(".apps-container");
        private static readonly By _appCards = By.CssSelector(".apps .app");
        private static readonly By _appName = By.CssSelector(".name");
        private static readonly By _appDescription = By.CssSelector(".description");
        private static readonly By _appDownloads = By.CssSelector(".downloads");
        private static readonly By _appDetailsLink = By.CssSelector("a[href*='app?title=']");
        private static readonly By _dumpLink = By.CssSelector(".footer a[href='/dump/']");
        private static readonly By _myAppsLink = By.CssSelector("a[href='/my']");
        private static readonly By _editAccountLink = By.CssSelector(".account a[href='/account']");
        private static readonly By _logoutLink = By.CssSelector(".account a[href*='logout']");

        public ApplicationsPage(IWebDriver driver) : base(driver)
        {
            // Можно добавить ожидание загрузки любого ключевого блока
            WaitHelper.WaitForElementVisible(Driver, _appsContainer);
        }

        // Header / account
        public IWebElement WelcomeLabel => Driver.FindElement(_welcome);
        public IWebElement LogoutLink => Driver.FindElement(_logoutLink);
        public IWebElement EditAccountLink => Driver.FindElement(_editAccountLink);
        public IWebElement MyApplicationsLink => ElementIfExists(_myAppsLink);
        public IWebElement DumpLink => Driver.FindElement(_dumpLink);

        // Коллекции
        public IReadOnlyCollection<IWebElement> PopularAppBlocks => Driver.FindElements(_popularApps);
        public IReadOnlyCollection<IWebElement> CategoryLinks => Driver.FindElements(_categoryLinks);
        public IReadOnlyCollection<IWebElement> AppCards => Driver.FindElements(_appCards);

       

        // Модель карточки (для удобства в тестах)
        public class AppCard
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int Downloads { get; set; }
            public IWebElement DetailsLink { get; set; }
        }

        public IEnumerable<AppCard> GetAppCardModels()
        {
            foreach (var card in AppCards)
            {
                var name = card.FindElement(_appName).Text.Trim();
                var desc = card.FindElement(_appDescription).Text.Trim();
                var downloadsText = card.FindElement(_appDownloads).Text.Trim(); // "# of downloads: X"
                int downloads = ParseDownloads(downloadsText);
                var details = card.FindElement(_appDetailsLink);
                yield return new AppCard
                {
                    Name = name,
                    Description = desc,
                    Downloads = downloads,
                    DetailsLink = details
                };
            }
        }

        private int ParseDownloads(string text)
        {
            // Ожидаемый формат: "# of downloads: N"
            var parts = text.Split(':');
            if (parts.Length == 2 && int.TryParse(parts[1].Trim(), out var n))
                return n;
            return 0;
        }

        // Навигация по категориям
        public void OpenCategory(string categoryName)
        {
            var link = CategoryLinks.FirstOrDefault(l => string.Equals(l.Text.Trim(), categoryName, StringComparison.OrdinalIgnoreCase));
            if (link == null)
                throw new NoSuchElementException($"Категория '{categoryName}' не найдена.");
            link.Click();
            // Ожидание обновления списка (можно ждать изменения урла или наличие первой карточки)
            WaitHelper.WaitUntil(Driver, d => d.Url.Contains("category=") || d.Url.EndsWith("/"));
            WaitHelper.WaitForElementVisible(Driver, _appsContainer);
        }

        // Открыть детали приложения по имени
        public void OpenAppDetails(string appName)
        {
            var card = AppCards.FirstOrDefault(c => c.FindElement(_appName).Text.Trim() == appName);
            if (card == null)
                throw new NoSuchElementException($"Приложение '{appName}' не найдено в видимом списке.");
            card.FindElement(_appDetailsLink).Click();
        }

        // Проверка наличия конкретного приложения (видимого)
        public bool IsAppVisible(string appName) =>
            AppCards.Any(c => c.FindElement(_appName).Text.Trim() == appName);

        // Получить число скачиваний конкретного приложения
        public int GetAppDownloads(string appName)
        {
            var card = AppCards.FirstOrDefault(c => c.FindElement(_appName).Text.Trim() == appName);
            if (card == null)
                throw new NoSuchElementException($"Карточка приложения '{appName}' не найдена.");
            return ParseDownloads(card.FindElement(_appDownloads).Text.Trim());
        }

        // Logout из страницы
        public LoginPage Logout()
        {
            LogoutLink.Click();
            return new LoginPage(Driver);
        }

        // Переход к моим приложениям (если ссылка существует)
        public ApplicationsPage OpenMyApplications()
        {
            var link = MyApplicationsLink ?? throw new NoSuchElementException("Ссылка 'My applications' отсутствует.");
            link.Click();
            WaitHelper.WaitUntil(Driver, d => d.Url.Contains("/my"));
            return new ApplicationsPage(Driver);
        }

        // Хелпер безопасного поиска
        private IWebElement ElementIfExists(By by)
        {
            var elements = Driver.FindElements(by);
            return elements.Count > 0 ? elements[0] : null;
        }
    }
//- Создан класс `ApplicationPage` наследник `BasePage`.
//- Вынесены локаторы в приватные `static readonly By`.
//- Добавлены методы получения популяных приложений, категорий, карточек, скачиваний.
//- Реализованы действия: `OpenCategory`, `OpenAppDetails`, `Logout`, `OpenMyApplications`.
//- Введена модель `AppCard` для удобной работы в тестах.
//- Используются существующие ожидания `WaitHelper` для стабильности.
}
