using log4net;
using NUnit.Framework;
using OpenQA.Selenium;
using Selenium.Framework;

namespace Selenium.Tests
{
    public abstract class BaseTest
    {
        protected IWebDriver Driver;
        protected ILog Logger;

        [SetUp]
        public virtual void Init()
        {
            this.Logger = LogManager.GetLogger(GetType());
            this.Logger.Info("log4net initialized");
            this.Driver = Settings.GetDriver();
            this.Driver.Manage().Window.Maximize();
            this.Logger.Info("Test started");
        }

        [TearDown]
        public virtual void Cleanup()
        {
            this.Driver.Quit();
        }
    }
}
