using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Pages
{
    public class ApplicationCard
    {
        // Драйвер и ожидание нужны, чтобы стабильно работать с элементами карточки.
        private readonly IWebDriver Driver;
        private readonly WebDriverWait Wait;

        // Локаторы элементов карточки приложения.
        private readonly By _appImage = By.CssSelector("img[alt^='Application']");
        private readonly By _appName = By.CssSelector("div.name");
        private readonly By _description = By.XPath("//div[@class='description' and contains(text(),'Description')]");
        private readonly By _category = By.XPath("//div[@class='description' and contains(text(),'Category')]");
        private readonly By _author = By.XPath("//div[@class='description' and contains(text(),'Author')]");
        private readonly By _downloads = By.CssSelector("div.downloads");

        // Локаторы кнопок действий.
        private readonly By _downloadButton = By.CssSelector("div.download-button a");
        private readonly By _deleteButton = By.CssSelector("div.edit-app-button a[href*='/delete']");
        private readonly By _editButton = By.CssSelector("div.edit-app-button a[href*='/edit']");

        // Локаторы формы рейтинга.
        private readonly By _rateDropdown = By.Name("rate");
        private readonly By _saveButton = By.CssSelector("input[type='submit'][name='save']");

        public ApplicationCard(IWebDriver driver)
        {
            // Проверяем входные данные, чтобы раньше увидеть проблему в тесте.
            Driver = driver ?? throw new ArgumentNullException(nameof(driver));
            Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // Свойства элементов карточки.
        public IWebElement AppImage { get { return Driver.FindElement(_appImage); } }
        public IWebElement AppName { get { return Driver.FindElement(_appName); } }
        public IWebElement Description { get { return Driver.FindElement(_description); } }
        public IWebElement Category { get { return Driver.FindElement(_category); } }
        public IWebElement Author { get { return Driver.FindElement(_author); } }
        public IWebElement Downloads { get { return Driver.FindElement(_downloads); } }

        // Свойства кнопок действий.
        public IWebElement DownloadButton { get { return Driver.FindElement(_downloadButton); } }
        public IWebElement DeleteButton { get { return Driver.FindElement(_deleteButton); } }
        public IWebElement EditButton { get { return Driver.FindElement(_editButton); } }

        // Свойства формы рейтинга.
        public IWebElement RateDropdown { get { return Driver.FindElement(_rateDropdown); } }
        public IWebElement SaveButton { get { return Driver.FindElement(_saveButton); } }

        // Скачиваем приложение.
        public void Download()
        {
            Wait.Until(d => DownloadButton.Displayed && DownloadButton.Enabled);
            DownloadButton.Click();
        }

        // Удаляем приложение.
        public void Delete()
        {
            Wait.Until(d => DeleteButton.Displayed && DeleteButton.Enabled);
            DeleteButton.Click();
        }

        // Переходим к редактированию приложения.
        public void Edit()
        {
            Wait.Until(d => EditButton.Displayed && EditButton.Enabled);
            EditButton.Click();
        }

        // Ставим оценку приложению и сохраняем.
        public void RateApp(int rating)
        {
            var select = new SelectElement(RateDropdown);
            select.SelectByValue(rating.ToString());
            SaveButton.Click();
        }

        // Методы чтения данных из карточки.
        public string GetAppName()
        {
            return AppName.Text.Trim();
        }

        public string GetDescription()
        {
            return Description.Text.Trim();
        }

        public string GetCategory()
        {
            return Category.Text.Trim();
        }

        public string GetAuthor()
        {
            return Author.Text.Trim();
        }

        public string GetDownloads()
        {
            return Downloads.Text.Trim();
        }
    }
}
