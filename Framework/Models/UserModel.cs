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
            var firstName = firstNames[random.Next(0, firstNames.Length)];
            var lastName = lastNames[random.Next(0, lastNames.Length)];
            var uniqueSuffix = Guid.NewGuid().ToString("N").Substring(0, 8);

            return new UserModel
            {
                Login = firstName + lastName + uniqueSuffix,
                Password = Guid.NewGuid().ToString(),
                FirstName = firstName,
                LastName = lastName
            };

        }
    }

    public class UserModelExtended : UserModel
    {
        public string Role { get; set; }
    }
}