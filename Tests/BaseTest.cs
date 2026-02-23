using System;
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
    }
}
