using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace Selenium.Pages
{
    public class EditApplicationsPage : BasePage
    {
        // Локаторы формы редактирования приложения
        private readonly By _pageHeader = By.XPath("//h1[normalize-space()='Edit application']");
        private readonly By _titleHidden = By.CssSelector("input[type='hidden'][name='title']");
        private readonly By _descriptionTextArea = By.Name("description");
        private readonly By _categorySelect = By.Name("category");
        private readonly By _imageUpload = By.Name("image");
        private readonly By _iconUpload = By.Name("icon");
        private readonly By _updateButton = By.CssSelector("input[type='submit'][value='Update']");


        public EditApplicationsPage(IWebDriver driver) : base(driver)
        {
            EnsureMyAppsPageLoaded();
        }

        private void EnsureMyAppsPageLoaded()
        {
            WaitHelper.WaitForElementVisible(Driver, _pageHeader);
            WaitHelper.WaitForElementVisible(Driver, _descriptionTextArea);
            WaitHelper.WaitForElementVisible(Driver, _updateButton);
        }

        // Элементы формы.
        public IWebElement PageHeader => Driver.FindElement(_pageHeader);
        public IWebElement TitleHidden => Driver.FindElement(_titleHidden);
        public IWebElement DescriptionTextArea => Driver.FindElement(_descriptionTextArea);
        public IWebElement CategorySelect => Driver.FindElement(_categorySelect);
        public IWebElement ImageUpload => Driver.FindElement(_imageUpload);
        public IWebElement IconUpload => Driver.FindElement(_iconUpload);
        public IWebElement UpdateButton => Driver.FindElement(_updateButton);

        // Методы взаимодействия с формой редактирования приложения
        public void EnterDescription(string description)
        {
            DescriptionTextArea.Clear();
            DescriptionTextArea.SendKeys(description);
        }

        public void SelectCategory(string categoryText)
        {
            var select = new SelectElement(CategorySelect);
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

        // Отправляем форму создания приложения.
        public ApplicationCard ClickUpdateButton()
        {
            UpdateButton.Click();
            return new ApplicationCard(Driver);
        }

        // Полный пошаговый сценарий заполнения формы редактирования.
        public void FillEditeApplicationForm(string description, string category, string imagePath = null, string iconPath = null)
        {
            EnterDescription(description);
            SelectCategory(category);

            if (!string.IsNullOrEmpty(imagePath))
                UploadImage(imagePath);

            if (!string.IsNullOrEmpty(iconPath))
                UploadIcon(iconPath);
        }
        // Методы чтения текущего состояния формы.
        public string GetCurrentTitle()
        {
            return TitleHidden.GetAttribute("value").Trim();
        }

        public string GetCurrentDescription()
        {
            return DescriptionTextArea.Text.Trim();
        }
        public string GetCurrentCategory()
        {
            var select = new SelectElement(CategorySelect);
            return select.SelectedOption.Text.Trim();
        }
        
    }
}