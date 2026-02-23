using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Selenium.Pages
{
    public abstract class BasePage
    {
        // Храним драйвер в базовом классе, чтобы все страницы использовали один и тот же браузер.
        protected IWebDriver Driver;

        protected BasePage(IWebDriver driver)
        {
            // Сохраняем экземпляр драйвера при создании любой страницы.
            Driver = driver;
        }

        // Локатор и элемент флеш-сообщения (успех/ошибка после действия).
        private readonly By _flashMessage = By.CssSelector(".flash");

        // Возвращаем текущий элемент флеш-сообщения.
        public IWebElement FlashMessage
        {
            get { return Driver.FindElement(_flashMessage); }
        }

        // Удобный метод для чтения текста флеш-сообщения в тестах.
        public string GetFlashMessage()
        {
            return FlashMessage.Text;
        }

        // Переход к объекту Header, чтобы удобно работать с шапкой сайта.
        public Header OnHeader()
        {
            return new Header(Driver);
        }

        public static class WaitHelper
        {
            // Базовое явное ожидание: ждём, пока условие не станет true.
            public static void WaitUntil(IWebDriver driver, Func<IWebDriver, bool> condition, int timeoutSeconds = 10)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
                wait.Until(condition);
            }

            // Ждём, пока элемент станет видимым, и возвращаем его.
            public static IWebElement WaitForElementVisible(IWebDriver driver, By locator, int timeoutSeconds = 10)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
                return wait.Until(ExpectedConditions.ElementIsVisible(locator));
            }

            // Ждём, пока элемент можно будет кликнуть.
            public static IWebElement WaitForElementClickable(IWebDriver driver, By locator, int timeoutSeconds = 10)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
                return wait.Until(ExpectedConditions.ElementToBeClickable(locator));
            }

            // Ждём, пока у элемента появится нужный текст.
            public static void WaitForTextContains(IWebDriver driver, By locator, string expectedText, int timeoutSeconds = 10)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));

                wait.Until(d =>
                {
                    try
                    {
                        var element = d.FindElement(locator);
                        return element.Text.Contains(expectedText);
                    }
                    catch (NoSuchElementException)
                    {
                        // Пока элемент не найден, продолжаем ждать.
                        return false;
                    }
                });
            }
        }
    }
}
