// Подключаем базовые типы .NET (StringComparison и др.).
using System;
// Подключаем коллекции и интерфейсы коллекций.
using System.Collections.Generic;
// Подключаем LINQ (FirstOrDefault, Any).
using System.Linq;
// Подключаем Selenium API.
using OpenQA.Selenium;

// Пространство имён page-object'ов.
namespace Selenium.Pages
{
    /// <summary>
    /// Page Object для страницы каталога приложений (Home / All / категории).
    /// </summary>
    public class ApplicationsPage : BasePage
    {
        // Локатор блока приветствия пользователя.
        private static readonly By _welcome = By.CssSelector(".welcome");
        // Локатор популярных приложений.
        private static readonly By _popularApps = By.CssSelector(".popular-container .popular-app");
        // Локатор ссылок категорий.
        private static readonly By _categoryLinks = By.CssSelector(".categories-ul li a");
        // Локатор контейнера списка приложений.
        private static readonly By _appsContainer = By.CssSelector(".apps-container");
        // Локатор карточек приложений.
        private static readonly By _appCards = By.CssSelector(".apps .app");
        // Локатор названия приложения внутри карточки.
        private static readonly By _appName = By.CssSelector(".name");
        // Локатор описания приложения внутри карточки.
        private static readonly By _appDescription = By.CssSelector(".description");
        // Локатор текста скачиваний внутри карточки.
        private static readonly By _appDownloads = By.CssSelector(".downloads");
        // Локатор ссылки деталей приложения внутри карточки.
        private static readonly By _appDetailsLink = By.CssSelector("a[href*='app?title=']");
        // Локатор служебной ссылки dump в футере.
        private static readonly By _dumpLink = By.CssSelector(".footer a[href='/dump/']");
        // Локатор ссылки "My applications" (доступна только определённым ролям).
        private static readonly By _myAppsLink = By.CssSelector("a[href='/my']");
        // Локатор ссылки редактирования аккаунта.
        private static readonly By _editAccountLink = By.CssSelector(".account a[href='/account']");
        // Локатор ссылки Logout.
        private static readonly By _logoutLink = By.CssSelector(".account a[href*='logout']");

        // Конструктор Page Object; принимает IWebDriver и передаёт в базовый класс.
        public ApplicationsPage(IWebDriver driver) : base(driver)
        {
            // Ожидаем видимость ключевого контейнера, чтобы страница считалась загруженной.
            WaitHelper.WaitForElementVisible(Driver, _appsContainer);
        }

        // Свойство: элемент приветствия в шапке.
        public IWebElement WelcomeLabel => Driver.FindElement(_welcome);
        // Свойство: элемент ссылки Logout.
        public IWebElement LogoutLink => Driver.FindElement(_logoutLink);
        // Свойство: элемент ссылки Edit account.
        public IWebElement EditAccountLink => Driver.FindElement(_editAccountLink);
        // Свойство: элемент ссылки My applications или null, если не найден.
        public IWebElement MyApplicationsLink => ElementIfExists(_myAppsLink);
        // Свойство: элемент ссылки dump.
        public IWebElement DumpLink => Driver.FindElement(_dumpLink);

        // Коллекция блоков популярных приложений.
        public IReadOnlyCollection<IWebElement> PopularAppBlocks => Driver.FindElements(_popularApps);
        // Коллекция ссылок категорий.
        public IReadOnlyCollection<IWebElement> CategoryLinks => Driver.FindElements(_categoryLinks);
        // Коллекция карточек приложений.
        public IReadOnlyCollection<IWebElement> AppCards => Driver.FindElements(_appCards);

        // Лёгкая модель карточки приложения для удобного чтения данных в тестах.
        public class AppCard
        {
            // Название приложения.
            public string Name { get; set; }
            // Описание приложения.
            public string Description { get; set; }
            // Количество скачиваний.
            public int Downloads { get; set; }
            // Ссылка на страницу деталей.
            public IWebElement DetailsLink { get; set; }
        }

        // Метод проецирует DOM-карточки в модели AppCard.
        public IEnumerable<AppCard> GetAppCardModels()
        {
            // Итерируемся по каждой карточке, найденной на странице.
            foreach (var card in AppCards)
            {
                // Возвращаем модель с извлечёнными полями.
                yield return new AppCard
                {
                    // Читаем и нормализуем название.
                    Name = card.FindElement(_appName).Text.Trim(),
                    // Читаем и нормализуем описание.
                    Description = card.FindElement(_appDescription).Text.Trim(),
                    // Парсим количество скачиваний из текста.
                    Downloads = ParseDownloads(card.FindElement(_appDownloads).Text.Trim()),
                    // Сохраняем ссылку на детали.
                    DetailsLink = card.FindElement(_appDetailsLink)
                };
            }
        }

        // Переход в указанную категорию приложений по имени.
        public void OpenCategory(string categoryName)
        {
            // Ищем ссылку категории без учёта регистра.
            var link = CategoryLinks.FirstOrDefault(l =>
                string.Equals(l.Text.Trim(), categoryName, StringComparison.OrdinalIgnoreCase));

            // Если категория не найдена — бросаем информативное исключение.
            if (link == null)
            {
                throw new NoSuchElementException($"Категория '{categoryName}' не найдена.");
            }

            // Кликаем по найденной категории.
            link.Click();
            // Ждём, что контейнер приложений отрисован после перехода/фильтрации.
            WaitHelper.WaitForElementVisible(Driver, _appsContainer);
        }

        // Открытие деталей приложения по имени карточки.
        public void OpenAppDetails(string appName)
        {
            // Ищем карточку с точным совпадением названия.
            var card = AppCards.FirstOrDefault(c => c.FindElement(_appName).Text.Trim() == appName);
            // Если карточки нет — бросаем исключение.
            if (card == null)
            {
                throw new NoSuchElementException($"Приложение '{appName}' не найдено в видимом списке.");
            }

            // Кликаем ссылку деталей внутри найденной карточки.
            card.FindElement(_appDetailsLink).Click();
        }

        // Быстрая проверка видимости приложения по имени.
        public bool IsAppVisible(string appName)
        {
            // Возвращаем true, если хотя бы одна карточка с таким именем найдена.
            return AppCards.Any(c => c.FindElement(_appName).Text.Trim() == appName);
        }

        // Получение количества скачиваний для конкретного приложения.
        public int GetAppDownloads(string appName)
        {
            // Ищем карточку приложения по имени.
            var card = AppCards.FirstOrDefault(c => c.FindElement(_appName).Text.Trim() == appName);
            // Если карточка отсутствует — ошибка.
            if (card == null)
            {
                throw new NoSuchElementException($"Карточка приложения '{appName}' не найдена.");
            }

            // Парсим число скачиваний из текстового блока карточки.
            return ParseDownloads(card.FindElement(_appDownloads).Text.Trim());
        }

        // Действие logout со страницы приложений.
        public LoginPage Logout()
        {
            // Кликаем ссылку Logout.
            LogoutLink.Click();
            // Возвращаем Page Object страницы логина.
            return new LoginPage(Driver);
        }

        // Переход на страницу "My applications".
        public MyApplicationsPage OpenMyApplications()
        {
            // Берём ссылку, если есть, иначе бросаем исключение.
            var link = MyApplicationsLink ?? throw new NoSuchElementException("Ссылка 'My applications' отсутствует.");
            // Кликаем по ссылке.
            link.Click();
            // Ждём URL целевой страницы.
            WaitHelper.WaitUntil(Driver, d => d.Url.Contains("/my"));
            // Возвращаем Page Object страницы "My applications".
            return new MyApplicationsPage(Driver);
        }

        // Вспомогательный парсер текста формата "# of downloads: N".
        private int ParseDownloads(string text)
        {
            // Делим строку по ':'.
            var parts = text.Split(':');
            // Если формат корректный и число распарсилось — возвращаем число, иначе 0.
            return parts.Length == 2 && int.TryParse(parts[1].Trim(), out var downloads) ? downloads : 0;
        }

        // Вспомогательный безопасный поиск элемента: возвращает null вместо исключения.
        private IWebElement ElementIfExists(By by)
        {
            // Получаем все совпавшие элементы.
            var elements = Driver.FindElements(by);
            // Если что-то найдено — возвращаем первый элемент, иначе null.
            return elements.Count > 0 ? elements[0] : null;
        }
    }
}
