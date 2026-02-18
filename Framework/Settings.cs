using System;
using System.IO;
using System.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace Selenium.Framework
{
    public class Settings
    {
        public static string GetBaseUrl()
        {
            return ConfigurationManager.AppSettings["baseUrl"];
        }

        public static IWebDriver GetDriver()
        {
            switch (GetBrowserType())
            {
                case "chrome":
                    return new ChromeDriver(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Drivers"));

                default:
                    throw new Exception("Unknown browser type!");
            }
            
        }

        public static string GetBrowserType()
        {
            return ConfigurationManager.AppSettings["browserType"];
        }
    }
}