using CRM.API.Models.InputModels;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CRM.IntegrationTests
{
    public class LeadInputModelMock
    {
        private static readonly object syncLock = new object();
        public static int RandomNumber(int min, int max)
        {
            Random rnd = new Random();
            lock (syncLock)
            {
                return rnd.Next(min, max);
            }
        }
        public static LeadInputModel leadInputModel = new LeadInputModel()
        {
            FirstName = "Регина",
            LastName = "Соловенько",
            Patronymic = "Дмитриевна",
            BirthDate = "12.02.1995",
            Email = $"reginaUberAlles@gmail.com{RandomNumber(1, 1000000000)}",
            Password = "fjdsfjeidfgdgfwoeifhw",
            Phone = $"+79995196034{RandomNumber(1, 1000000000)}",
            Login = $"regina{RandomNumber(1, 1000000000)}",
            CityId = 3
        };

        public static List<LeadInputModel> leadsToInsertForSearchTest = new List<LeadInputModel>()
        {
            new LeadInputModel
            {
                FirstName = "Азалия",
                LastName = "Короткова",
                Patronymic = "Иннокентьевна",
                BirthDate = "02.01.1990",
                Email = $"AsalKorot@gmail.com",
                Password = "notDifficultSuperPassword",
                Phone = $"+79941028872",
                Login = $"AsalKorot1990",
                CityId = 3
            },
            new LeadInputModel
            {
                FirstName = "Виталия",
                LastName = "Оборотова",
                Patronymic = "Васильевна",
                BirthDate = "02.01.1991",
                Email = $"VitOborot@gmail.com",
                Password = "superDifficultPassword",
                Phone = $"+79941028876",
                Login = $"VitOborot2000",
                CityId = 1
            },
            new LeadInputModel
            {
                FirstName = "Джулия",
                LastName = "Воротникова",
                Patronymic = "Геннадьевна",
                BirthDate = "02.01.1992",
                Email = $"JulVorot@gmail.com",
                Password = "difficultSuperPassword",
                Phone = $"+79941028871",
                Login = $"JulVorot1980",
                CityId = 2
            }
        };
    }
}
