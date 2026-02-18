using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Selenium.Framework.Models
{
    public class UserModel
    {
        public string Login { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
        public string ConfirmPassword { get; set; }

        public static UserModel GetDefaultUser()
        {
            return new UserModel { Login = "admin", Password = "admin", FirstName = "Ivan", LastName = "Petrov", ConfirmPassword = "admin" };
        }

        // возможный вариант генерации нового Юзера - после чего он будет отправлен на регистрацию
        private static string[] firstNames = new string[]
        {
            "Jack","Mike", "Jane", "Fred", "Helen"

        };

        private static string[] lastNames = new string[]
        {
            "Jackson","Cartwright", "Mayert", "Haley", "Wolter"

        };
        public static UserModel GetRandom()
        {
            Random random = new Random();
            return new UserModel
            {
                Login = firstNames[random.Next(0, firstNames.Length - 1)] + lastNames[random.Next(0, lastNames.Length - 1)],
                Password = Guid.NewGuid().ToString(),
                FirstName = firstNames[random.Next(0, firstNames.Length - 1)],
                LastName = lastNames[random.Next(0, lastNames.Length - 1)],
            };

        }
    }

    public class UserModelExtended : UserModel
    {
        public string Role { get; set; }
    }
}