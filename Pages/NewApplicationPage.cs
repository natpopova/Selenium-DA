using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Selenium.Pages
{
    public class NewApplicationPage : BasePage
    {
        public NewApplicationPage(IWebDriver driver) : base(driver)
        {
        }

        // 🔹 Заголовок страницы
        public IWebElement Header => Driver.FindElement(By.TagName("h1"));

        // 🔹 Поле ввода названия приложения
        public IWebElement TitleInput => Driver.FindElement(By.Name("title"));

        // 🔹 Поле ввода описания
        public IWebElement DescriptionTextArea => Driver.FindElement(By.Name("description"));

        // 🔹 Выпадающий список категорий
        public IWebElement CategorySelect => Driver.FindElement(By.Name("category"));

        // 🔹 Поле загрузки изображения (512x512)
        public IWebElement ImageUpload => Driver.FindElement(By.Name("image"));

        // 🔹 Поле загрузки иконки (128x128)
        public IWebElement IconUpload => Driver.FindElement(By.Name("icon"));

        // 🔹 Кнопка создания приложения
        public IWebElement CreateButton => Driver.FindElement(By.CssSelector("input[type='submit'][value='Create']"));

        // 🧩 Методы для взаимодействия с элементами
        public void EnterTitle(string title)
        {
            TitleInput.Clear();
            TitleInput.SendKeys(title);
        }

        public void EnterDescription(string description)
        {
            DescriptionTextArea.Clear();
            DescriptionTextArea.SendKeys(description);
        }

        public void SelectCategory(string categoryText)
        {
            var select = new OpenQA.Selenium.Support.UI.SelectElement(CategorySelect);
            select.SelectByText(categoryText);
        }

        public void UploadImage(string imagePath)
        {
            ImageUpload.SendKeys(imagePath);
        }

        public void UploadIcon(string iconPath)
        {
            IconUpload.SendKeys(iconPath);
        }

        public void ClickCreateButton()
        {
            CreateButton.Click();
        }

        // Полный сценарий заполнения формы
        public void FillNewApplicationForm(string title, string description, string category, string imagePath, string iconPath)
        {
            EnterTitle(title);
            EnterDescription(description);
            SelectCategory(category);
            UploadImage(imagePath);
            UploadIcon(iconPath);
        }
    }

}

