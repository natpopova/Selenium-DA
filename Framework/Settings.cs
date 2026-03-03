using System;
using System.Configuration;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace Selenium.Framework
{
    public class Settings
    {
        // Базовый URL берём из App.config, чтобы не дублировать в коде.
        public static string GetBaseUrl()
        {
            return ConfigurationManager.AppSettings["baseUrl"];
        }

        // Тип браузера также читаем из конфигурации.
        public static string GetBrowserType()
        {
            return ConfigurationManager.AppSettings["browserType"];
        }

        // Центральный метод создания веб-драйвера.
        public static IWebDriver GetDriver()
        {
            var browserType = GetBrowserType();

            if (string.Equals(browserType, "chrome", StringComparison.OrdinalIgnoreCase))
            {
                var options = new ChromeOptions();
                options.AddArgument("--start-maximized");
                options.AddArgument("--disable-notifications");
                options.AddArgument("--disable-popup-blocking");

                return new ChromeDriver(options); // Selenium Manager сам скачает нужный драйвер
            }

            throw new Exception("Unknown browser type: " + browserType);
        }
    }
}
