using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;

namespace Selenium.Pages
{
    public class AJAXPage : BasePage
    {

        //locators for input fields, button and result

        private readonly By _xInput = By.Id("x");
        private readonly By _yInput = By.Id("y");   
        private readonly By _sumButton= By.Id("calc");
        private readonly By _clearButton = By.Id("clear");
        private readonly By _result = By.Id("result");

        //elements as properties


        public AJAXPage(IWebDriver driver) : base(driver)
        {
            EnsureLoaded();
        }

        public void ClickSumButton()
        {
            var sum = Driver.FindElement(_sumButton);
            sum.Click();
        }

        public void EnterFirstNumber(string value)
        {
            var x = Driver.FindElement(_xInput);
            x.Clear();
            x.SendKeys(value);
        }

        public void EnterSecondNumber(string value)
        {
            var y= Driver.FindElement(_yInput);
            y.Clear();
            y.SendKeys(value);
        }

        public string WaitForResultTextContains(string expectedPart, int timeoutSec = 10)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSec));
            wait.Until(d =>
            {
                var resultElement = GetResultText();
                return !string.IsNullOrWhiteSpace(resultElement) 
                        && resultElement.IndexOf(expectedPart, StringComparison.OrdinalIgnoreCase) >= 0;
            });
        }

        public string GetResultText()
        {
            return Driver.FindElement(_result).Text.Trim();
        }

        public void EnsureLoaded()
        {
            WaitHelper.WaitForElementVisible(Driver, By.Id("x"));
            WaitHelper.WaitForElementVisible(Driver, By.Id("y"));
            WaitHelper.WaitForElementVisible(Driver, By.Id("calc"));
        }
    }
}