using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Selenium.Pages
{
    public abstract class BasePage
    {
        public IWebDriver Driver;
        

        public BasePage(IWebDriver driver)
        {
            this.Driver = driver;
        }

        public IWebElement FlashMessage => Driver.FindElement(By.CssSelector(".flash"));

        public string GetFlashMessage() => FlashMessage.Text;

        public Header OnHeader()
        {
            return new Header(Driver);
        }

        public static class WaitHelper
        {
            /// Явное ожидание до тех пор, пока функция вернёт true.
            public static void WaitUntil(IWebDriver driver, Func<IWebDriver, bool> condition, int timeoutSeconds = 10)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
                wait.Until(condition);
            }

            /// Ожидание видимости элемента по локатору.
            public static IWebElement WaitForElementVisible(IWebDriver driver, By locator, int timeoutSeconds = 10)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
                return wait.Until(ExpectedConditions.ElementIsVisible(locator));
            }

            /// Ожидание, пока элемент станет кликабельным.
            public static IWebElement WaitForElementClickable(IWebDriver driver, By locator, int timeoutSeconds = 10)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutSeconds));
                return wait.Until(ExpectedConditions.ElementToBeClickable(locator));
            }

            /// Ожидание, пока текст элемента содержит определённое значение.
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
                        return false;
                    }
                });
            }


        }
    }
}