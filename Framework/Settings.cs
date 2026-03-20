using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using System;
using System.Configuration;


namespace Selenium.Framework
{
    public class Settings
    {
        // Базовый URL берём из App.config
        public static string GetBaseUrl()
        {
            return ConfigurationManager.AppSettings["baseUrl"];
        }

        //читают из appSettings значения для Basic Auth username/password
        public static string GetBasicAuthUsername()
        {
            return ConfigurationManager.AppSettings["basicAuthUsername"];
        }

        public static string GetBasicAuthPassword()
        {
            return ConfigurationManager.AppSettings["basicAuthPassword"];
        }


        // Собираем абсолютный URL
        //http://username:password@selenium-courses.ipa.dataart.net:8081/

        public static string BuildUrl(string relativePath = "")
        {
            var baseUri = new Uri(GetBaseUrl().TrimEnd('/') + "/");
            var targetUri = new Uri(baseUri, relativePath.TrimStart('/'));

            var username = GetBasicAuthUsername();
            var password = GetBasicAuthPassword();

            if (string.IsNullOrWhiteSpace(username))
            {
                return targetUri.ToString();
            }

            var uriBuilder = new UriBuilder(targetUri)
            {
                UserName = username,
                Password = password ?? string.Empty
            };

            return uriBuilder.Uri.ToString();
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
            if (string.Equals(browserType, "firefox", StringComparison.OrdinalIgnoreCase))
            {
                var options = new FirefoxOptions();
                options.AddArgument("--width=1920");
                options.AddArgument("--height=1080");

                return new FirefoxDriver(options); // Selenium Manager сам скачает нужный драйвер
            }
            if (string.Equals(browserType, "edge", StringComparison.OrdinalIgnoreCase))
            {
                var options = new EdgeOptions();
                options.AddArgument("--start-maximized");

                return new EdgeDriver(options);
            }

            throw new Exception("Unknown browser type: " + browserType);
        }
    }
}
