using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace Selenium.Pages
{
    public class NewApplicationPage : BasePage
    {
        // Локаторы элементов формы создания приложения.
        private readonly By _header = By.TagName("h1");
        private readonly By _titleInput = By.Name("title");
        private readonly By _descriptionTextArea = By.Name("description");
        private readonly By _categorySelect = By.Name("category");
        private readonly By _imageUpload = By.Name("image");
        private readonly By _iconUpload = By.Name("icon");
        private readonly By _createButton = By.CssSelector("input[type='submit'][value='Create']");

        public NewApplicationPage(IWebDriver driver) : base(driver)
        {
        }

        // Элементы формы.
        public IWebElement Header => Driver.FindElement(_header);
        public IWebElement TitleInput => Driver.FindElement(_titleInput);
        public IWebElement DescriptionTextArea => Driver.FindElement(_descriptionTextArea);
        public IWebElement CategorySelect => Driver.FindElement(_categorySelect);
        public IWebElement ImageUpload => Driver.FindElement(_imageUpload);
        public IWebElement IconUpload => Driver.FindElement(_iconUpload);
        public IWebElement CreateButton => Driver.FindElement(_createButton);

        // Вводим название приложения.
        public void EnterTitle(string title)
        {
            TitleInput.Clear();
            TitleInput.SendKeys(title);
        }

        // Вводим описание приложения.
        public void EnterDescription(string description)
        {
            DescriptionTextArea.Clear();
            DescriptionTextArea.SendKeys(description);
        }

        // Выбираем категорию из выпадающего списка.
        public void SelectCategory(string categoryText)
        {
            var select = new SelectElement(CategorySelect);
            select.SelectByText(categoryText);
        }

        // Загружаем основную картинку приложения.
        public void UploadImage(string imagePath)
        {
            ImageUpload.SendKeys(imagePath);
        }

        // Загружаем иконку приложения.
        public void UploadIcon(string iconPath)
        {
            IconUpload.SendKeys(iconPath);
        }

        // Отправляем форму создания приложения.
        public void ClickCreateButton()
        {
            CreateButton.Click();
        }

        // Полный пошаговый сценарий заполнения формы.
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
