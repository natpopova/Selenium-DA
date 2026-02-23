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

        // Элементы шапки.
        public IWebElement WelcomeLabel { get { return Driver.FindElement(_welcome); } }
        public IWebElement LogoutLink { get { return Driver.FindElement(_logoutLink); } }
        public IWebElement EditAccountLink { get { return Driver.FindElement(_editAccountLink); } }
        public IWebElement DumpLink { get { return Driver.FindElement(_dumpLink); } }

        // Ссылка может отсутствовать для некоторых ролей, поэтому используем безопасный поиск.
        public IWebElement MyApplicationsLink { get { return ElementIfExists(_myAppsLink); } }

        // Коллекции элементов на странице.
        public IReadOnlyCollection<IWebElement> PopularAppBlocks { get { return Driver.FindElements(_popularApps); } }
        public IReadOnlyCollection<IWebElement> CategoryLinks { get { return Driver.FindElements(_categoryLinks); } }
        public IReadOnlyCollection<IWebElement> AppCards { get { return Driver.FindElements(_appCards); } }

        // Простая модель карточки для удобства в проверках.
        public class AppCard
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int Downloads { get; set; }
            public IWebElement DetailsLink { get; set; }
        }

        // Читаем все карточки и возвращаем их как список моделей.
        public IEnumerable<AppCard> GetAppCardModels()
        {
            foreach (var card in AppCards)
            {
                var model = new AppCard();

                // Читаем название.
                model.Name = card.FindElement(_appName).Text.Trim();

                // Читаем описание.
                model.Description = card.FindElement(_appDescription).Text.Trim();

                // Читаем и парсим количество скачиваний.
                var downloadsText = card.FindElement(_appDownloads).Text.Trim();
                model.Downloads = ParseDownloads(downloadsText);

                // Сохраняем ссылку на детали.
                model.DetailsLink = card.FindElement(_appDetailsLink);

                yield return model;
            }
        }

        // Проверяем, есть ли категория с указанным названием.
        public bool HasCategory(string categoryName)
        {
            return CategoryLinks.Any(link => link.Text.Trim().Equals(categoryName, StringComparison.OrdinalIgnoreCase));
        }

        // Нажимаем на категорию по её тексту.
        public void OpenCategory(string categoryName)
        {
            var categoryLink = CategoryLinks.FirstOrDefault(link => link.Text.Trim().Equals(categoryName, StringComparison.OrdinalIgnoreCase));

            if (categoryLink == null)
            {
                throw new NoSuchElementException("Категория '" + categoryName + "' не найдена.");
            }

            categoryLink.Click();
        }

        // Проверяем наличие приложения по имени.
        public bool HasApplication(string appName)
        {
            return AppCards.Any(card => card.FindElement(_appName).Text.Trim().Equals(appName, StringComparison.OrdinalIgnoreCase));
        }

        // Открываем страницу деталей приложения.
        public void OpenApplicationDetails(string appName)
        {
            var targetCard = AppCards.FirstOrDefault(card => card.FindElement(_appName).Text.Trim().Equals(appName, StringComparison.OrdinalIgnoreCase));

            if (targetCard == null)
            {
                throw new NoSuchElementException("Карточка приложения '" + appName + "' не найдена.");
            }

            targetCard.FindElement(_appDetailsLink).Click();
        }

        // Возвращаем число скачиваний для конкретного приложения.
        public int GetDownloadsForApp(string appName)
        {
            var targetCard = AppCards.FirstOrDefault(card => card.FindElement(_appName).Text.Trim().Equals(appName, StringComparison.OrdinalIgnoreCase));

            if (targetCard == null)
            {
                throw new NoSuchElementException("Карточка приложения '" + appName + "' не найдена.");
            }

            var downloadsText = targetCard.FindElement(_appDownloads).Text.Trim();
            return ParseDownloads(downloadsText);
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
