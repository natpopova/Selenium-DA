using OpenQA.Selenium;
using System;
using System.Linq;

namespace Selenium.Pages
{
    public class MyApplicationsPage : BasePage
    {
        // Локатор ссылки для добавления нового приложения.
        private readonly By _addNewApplicationLink = By.PartialLinkText("new");
        private readonly By _appDetailsLink = By.CssSelector("a[href*='/app?title=']");
        private readonly By _appCardsContainer = By.CssSelector(".apps");
        private readonly By _appCards = By.CssSelector(".apps .app");
        private readonly By _appName = By.CssSelector(".name");

        public MyApplicationsPage(IWebDriver driver) : base(driver)
        {
            EnsureMyAppsPageLoaded();
        }

        private void EnsureMyAppsPageLoaded()
        {
            WaitHelper.WaitUntil(Driver, d => d.Url.Contains("/my"));
            WaitHelper.WaitUntil(Driver, d =>
                d.FindElements(_addNewApplicationLink).Count > 0 ||
                d.FindElements(_appCardsContainer).Count > 0);
        }

        //// Возвращаем ссылку "Add new application".
        //public IWebElement AddNewApplicationLink => Driver.FindElement(_addNewApplicationLink);

        // Открываем форму создания нового приложения.
        public NewApplicationPage OpenNewApplicationForm()
        {
            WaitHelper.WaitForElementClickable(Driver, _addNewApplicationLink).Click();
            return new NewApplicationPage(Driver);
        }

        // Находим карточку приложения по точному имени.
        public IWebElement FindAppCardByName(string appName, int timeoutSeconds = 20)
        {
            // ждём пока появится нужная карточка
            WaitHelper.WaitUntil(Driver, d =>
            {
                return d.FindElements(_appCards)
                        .Any(card => card.FindElement(_appName)
                        .Text.Trim() == appName);
            }, timeoutSeconds);

            // после ожидания ищем и возвращаем карточку
            var cards = Driver.FindElements(_appCards)
                                .FirstOrDefault(card => card.FindElement(_appName)
                                .Text.Trim() == appName);
            if (cards == null)
                throw new NoSuchElementException($"Приложение с именем '{appName}' не найдено.");

            return cards;

        }

        // Открываем страницу деталей приложения из списка "My applications" по имени.
        public ApplicationCard OpenAppDetailsByName(string appName)
        {
            var appCard = FindAppCardByName(appName);
            var detailsLink = appCard.FindElement(_appDetailsLink);
            detailsLink.Click();
            return new ApplicationCard(Driver);
        }
    }
}