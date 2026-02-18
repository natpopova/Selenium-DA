using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace Selenium.Pages
{
    public class MyApplicationsPage : BasePage
    {
        public MyApplicationsPage(IWebDriver driver) : base(driver)
        {
        }

        public IWebElement AddNewApplicationLink => Driver.FindElement(By.PartialLinkText("new"));

    }
}
