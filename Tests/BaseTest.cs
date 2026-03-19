using log4net;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using Selenium.Framework;
using System;
using System.IO;

namespace Selenium.Tests
{
    public abstract class BaseTest
    {
        protected IWebDriver Driver;
        protected ILog Logger;

        [SetUp]
        public virtual void Init()
        {
            // Инициализируем логгер для удобной диагностики тестов.
            Logger = LogManager.GetLogger(GetType());
            Logger.Info("log4net initialized");

            // Создаём браузер через общий Settings, чтобы все тесты стартовали одинаково.
            Driver = Settings.GetDriver();

            // Небольшой implicit wait снижает количество флак при динамических элементах.
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

            Logger.Info("Test started");
        }

        [TearDown]
        public virtual void Cleanup()
        {
            SaveScreenshotOnFailure();

            // Всегда закрываем браузер после теста, чтобы не копить процессы.
            try
            {
                Driver?.Quit();
            }
            catch
            {
                Driver?.Dispose();
            }
        }

        public void SaveScreenshotOnFailure()
        {
            if (Driver == null || TestContext.CurrentContext.Result.Outcome.Status != TestStatus.Failed)
            {
                return;
            }

            if (!(Driver is ITakesScreenshot screenshotDriver))
            {
                return;
            }

            var screenshotsDirectory = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Screenshots");
            Directory.CreateDirectory(screenshotsDirectory);

            var fileName = $"{GetSafeTestName()}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.png";
            var screenshotPath = Path.Combine(screenshotsDirectory, fileName);

            screenshotDriver.GetScreenshot().SaveAsFile(screenshotPath);
            TestContext.AddTestAttachment(screenshotPath, "Failure screenshot");
        }

        private static string GetSafeTestName()
        {
            var testName = TestContext.CurrentContext.Test.Name;
            foreach (var invalidCharacter in Path.GetInvalidFileNameChars())
            {
                testName = testName.Replace(invalidCharacter, '_');
            }

            return testName;
        }
    }
}
