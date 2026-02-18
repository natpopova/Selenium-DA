using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Pages
{
    public class ApplicationCard
    {
        private readonly IWebDriver Driver;
        private readonly WebDriverWait Wait;

        public ApplicationCard(IWebDriver driver)
        {
            Driver = driver ?? throw new ArgumentNullException(nameof(driver));
            Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        // ---------- Локаторы ----------

        // Картинка приложения
        public IWebElement AppImage => Driver.FindElement(By.CssSelector("img[alt^='Application']"));

        // Название
        public IWebElement AppName => Driver.FindElement(By.CssSelector("div.name"));

        // Описание
        public IWebElement Description => Driver.FindElement(By.XPath("//div[@class='description' and contains(text(),'Description')]"));

        // Категория
        public IWebElement Category => Driver.FindElement(By.XPath("//div[@class='description' and contains(text(),'Category')]"));

        // Автор
        public IWebElement Author => Driver.FindElement(By.XPath("//div[@class='description' and contains(text(),'Author')]"));

        // Количество загрузок
        public IWebElement Downloads => Driver.FindElement(By.CssSelector("div.downloads"));

        // Кнопки действий
        public IWebElement DownloadButton => Driver.FindElement(By.CssSelector("div.download-button a"));
        public IWebElement DeleteButton => Driver.FindElement(By.CssSelector("div.edit-app-button a[href*='/delete']"));
        public IWebElement EditButton => Driver.FindElement(By.CssSelector("div.edit-app-button a[href*='/edit']"));

        // Форма рейтинга
        public IWebElement RateDropdown => Driver.FindElement(By.Name("rate"));
        public IWebElement SaveButton => Driver.FindElement(By.CssSelector("input[type='submit'][name='save']"));

        // ---------- Методы действий ----------

        public void Download()
        {
            Wait.Until(d => DownloadButton.Displayed && DownloadButton.Enabled);
            DownloadButton.Click();
        }

        public void Delete()
        {
            Wait.Until(d => DeleteButton.Displayed && DeleteButton.Enabled);
            DeleteButton.Click();
     
        }

        public void Edit()
        {
            Wait.Until(d => EditButton.Displayed && EditButton.Enabled);
            EditButton.Click();
        }

        public void RateApp(int rating)
        {
            var select = new SelectElement(RateDropdown);
            select.SelectByValue(rating.ToString());
            SaveButton.Click();
        }

        // ---------- Проверки ----------

        public string GetAppName() => AppName.Text.Trim();
        public string GetDescription() => Description.Text.Trim();
        public string GetCategory() => Category.Text.Trim();
        public string GetAuthor() => Author.Text.Trim();
        public string GetDownloads() => Downloads.Text.Trim();
    }
}

