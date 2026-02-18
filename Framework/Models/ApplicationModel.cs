using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Selenium.Framework.Models;

namespace Selenium.Framework.Models
{
    public class ApplicationModel
    {
        public string Title { get; set; }              // Название приложения (например, "Application Fun 2")
        public string Description { get; set; }        // Описание
        public string Category { get; set; }           // Категория (например, "Fun")
        public string Author { get; set; }             // Автор (например, "admin")
        public int DownloadsCount { get; set; }        // Кол-во скачиваний
        public double AverageRate { get; set; }        // Средний рейтинг

        // Дополнительно (если нужно использовать в контроллерах):
        public string ImageUrl { get; set; }           // Путь к изображению (/image?id=...)
        public string DownloadUrl { get; set; }        // Ссылка на загрузку (/download?title=...)
        public string EditUrl { get; set; }            // Ссылка на редактирование (/edit?title=...)
        public string DeleteUrl { get; set; }          // Ссылка на удаление (/delete?title=...)
    }

}
//Пример заполнения модели
//var app = new ApplicationModel
//{
//    Title = "Application Fun 2",
//    Description = "This description is edited",
//    Category = "Fun",
//    Author = "admin",
//    DownloadsCount = 1,
//    AverageRate = 0.0,
//    ImageUrl = "/image?id=Application Fun 2",
//    DownloadUrl = "/download?title=Application Fun 2",
//    EditUrl = "/edit?title=Application Fun 2",
//    DeleteUrl = "/delete?title=Application Fun 2"
//};
