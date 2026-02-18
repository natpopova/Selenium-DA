using System;
using log4net;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace Selenium.Tests
{
    public abstract class BaseTest
    {
        protected IWebDriver Driver;
        protected ILog Logger;

        [SetUp]
        public virtual void Init()
        {
            Logger = LogManager.GetLogger(GetType());
            Logger.Info("log4net initialized");

            // ➊ Авто-скачивание совместимого chromedriver
            new DriverManager().SetUpDriver(new ChromeConfig());

            // ➋ Опции браузера (можно дополнить при необходимости)
            var options = new ChromeOptions();
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--disable-popup-blocking");

            // ➌ Создание драйвера без явного пути
            Driver = new ChromeDriver(options);

            // ➍ Базовые таймауты (по желанию)
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);

            Logger.Info("Test started");
        }

        [TearDown]
        public virtual void Cleanup()
        {
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