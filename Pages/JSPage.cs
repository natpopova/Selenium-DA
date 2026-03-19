using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;

namespace Selenium.Pages
{
    public class JSPage : BasePage
    {

        //locators for input fields, button and result

        private readonly By _topInput = By.Id("top");
        private readonly By _leftInput = By.Id("left");   
        private readonly By _processButton= By.Id("process");
        private readonly By _jumpingDiv = By.CssSelector(".flash");



        public JSPage(IWebDriver driver) : base(driver)
        {
            EnsureLoaded();
        }

        public string GetAndAcceptAlertText(int timeoutSeconds = 10)
        {
            var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
            var alert = wait.Until(ExpectedConditions.AlertIsPresent());
            var alertText = alert.Text;
            alert.Accept();

            return alertText;
        }

        public void ClickProcessButton()
        {
            var sum = Driver.FindElement(_processButton);
            sum.Click();
        }

        public void EnterLeftCoordinate(int left)
        {
            var input = Driver.FindElement(_leftInput);
            input.Clear();
            input.SendKeys(left.ToString());
        }
        public void EnterTopCoordinate(int top)
        {
            var input = Driver.FindElement(_topInput);
            input.Clear();
            input.SendKeys(top.ToString());
        }

        public void EnsureLoaded()
        {
            WaitHelper.WaitForElementVisible(Driver, By.Id("process"));
        }

        public int[] GetJumpingCoordinates()
        {
            var js = (IJavaScriptExecutor)Driver;

            var result = js.ExecuteScript(@"
        var el = document.querySelector('.flash');
        if (!el) return null;
        var r = el.getBoundingClientRect();
        return [ Math.round(r.top + window.pageYOffset), Math.round(r.left + window.pageXOffset) ];
    ") as System.Collections.ObjectModel.ReadOnlyCollection<object>;

            if (result == null || result.Count != 2)
                throw new InvalidOperationException("Could not read coordinates of .flash element.");

            return new[] { Convert.ToInt32(result[0]), Convert.ToInt32(result[1]) };
        }
    }
}