using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;

namespace Selenium.Pages
{
    /// <summary>
    /// Страница каталога приложений (Home / All / категории).
    /// </summary>
    public class ApplicationsPage : BasePage
    {
        private static readonly By _welcome = By.CssSelector(".welcome");
        private static readonly By _popularApps = By.CssSelector(".popular-container .popular-app");
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
            WaitHelper.WaitForElementVisible(Driver, _appsContainer);
        }

        public IWebElement WelcomeLabel => Driver.FindElement(_welcome);
        public IWebElement LogoutLink => Driver.FindElement(_logoutLink);
        public IWebElement EditAccountLink => Driver.FindElement(_editAccountLink);
        public IWebElement MyApplicationsLink => ElementIfExists(_myAppsLink);
        public IWebElement DumpLink => Driver.FindElement(_dumpLink);

        public IReadOnlyCollection<IWebElement> PopularAppBlocks => Driver.FindElements(_popularApps);
        public IReadOnlyCollection<IWebElement> CategoryLinks => Driver.FindElements(_categoryLinks);
        public IReadOnlyCollection<IWebElement> AppCards => Driver.FindElements(_appCards);

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
                yield return new AppCard
                {
                    Name = card.FindElement(_appName).Text.Trim(),
                    Description = card.FindElement(_appDescription).Text.Trim(),
                    Downloads = ParseDownloads(card.FindElement(_appDownloads).Text.Trim()),
                    DetailsLink = card.FindElement(_appDetailsLink)
                };
            }
        }

        public void OpenCategory(string categoryName)
        {
            var link = CategoryLinks.FirstOrDefault(l =>
                string.Equals(l.Text.Trim(), categoryName, StringComparison.OrdinalIgnoreCase));

            if (link == null)
            {
                throw new NoSuchElementException($"Категория '{categoryName}' не найдена.");
            }

            link.Click();
            WaitHelper.WaitForElementVisible(Driver, _appsContainer);
        }

        public void OpenAppDetails(string appName)
        {
            var card = AppCards.FirstOrDefault(c => c.FindElement(_appName).Text.Trim() == appName);
            if (card == null)
            {
                throw new NoSuchElementException($"Приложение '{appName}' не найдено в видимом списке.");
            }

            card.FindElement(_appDetailsLink).Click();
        }

        public bool IsAppVisible(string appName)
        {
            return AppCards.Any(c => c.FindElement(_appName).Text.Trim() == appName);
        }

        public int GetAppDownloads(string appName)
        {
            var card = AppCards.FirstOrDefault(c => c.FindElement(_appName).Text.Trim() == appName);
            if (card == null)
            {
                throw new NoSuchElementException($"Карточка приложения '{appName}' не найдена.");
            }

            return ParseDownloads(card.FindElement(_appDownloads).Text.Trim());
        }

        public LoginPage Logout()
        {
            LogoutLink.Click();
            return new LoginPage(Driver);
        }

        public MyApplicationsPage OpenMyApplications()
        {
            var link = MyApplicationsLink ?? throw new NoSuchElementException("Ссылка 'My applications' отсутствует.");
            link.Click();
            WaitHelper.WaitUntil(Driver, d => d.Url.Contains("/my"));
            return new MyApplicationsPage(Driver);
        }

        private int ParseDownloads(string text)
        {
            var parts = text.Split(':');
            return parts.Length == 2 && int.TryParse(parts[1].Trim(), out var downloads) ? downloads : 0;
        }

        private IWebElement ElementIfExists(By by)
        {
            var elements = Driver.FindElements(by);
            return elements.Count > 0 ? elements[0] : null;
        }
    }
}
