using System;
using System.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

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
                // Важно: скачиваем совместимый chromedriver под установленную версию Chrome.
                // Это устраняет ошибку вида:
                // "This version of ChromeDriver only supports Chrome version X; Current browser version is Y".
                new DriverManager().SetUpDriver(new ChromeConfig());

                // Базовые опции для стабильного запуска автотестов.
                var options = new ChromeOptions();
                options.AddArgument("--start-maximized");
                options.AddArgument("--disable-notifications");
                options.AddArgument("--disable-popup-blocking");

                // Не указываем путь к фиксированному драйверу из папки Drivers,
                // чтобы использовать актуальный драйвер, установленный WebDriverManager.
                return new ChromeDriver(options);
            }

            throw new Exception("Unknown browser type: " + browserType);
        }
    }
}
