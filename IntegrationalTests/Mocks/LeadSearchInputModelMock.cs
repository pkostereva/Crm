using CRM.API.Models.InputModels;
using System;

namespace CRM.IntegrationTests.Mocks
{
    public class LeadSearchInputModelMock
    {
        public static LeadSearchInputModel searchNothing = new LeadSearchInputModel()
        {
            FirstName = "Агния"
        };

        public static LeadSearchInputModel searchByFirstNameOperation = new LeadSearchInputModel()
        {
            FirstName = "%лия",
            FirstNameOperator = 3
        };

        public static LeadSearchInputModel searchByLastNameOperation = new LeadSearchInputModel()
        {
            LastName = "%орот%",
            LastNameOperator = 3
        };

        public static LeadSearchInputModel searchByPatronymicOperation = new LeadSearchInputModel()
        {
            Patronymic = "%нн%",
            PatronymicOperator = 3
        };

        public static LeadSearchInputModel searchByBirthDateMore = new LeadSearchInputModel()
        {
            BirthDate = "01.01.1990",
            BirthDateOperator = 2
        };

        public static LeadSearchInputModel searchByBirthDateLess = new LeadSearchInputModel()
        {
            BirthDate = "03.01.1992",
            BirthDateOperator = 3
        };

        public static LeadSearchInputModel searchByBirthDateBetween = new LeadSearchInputModel()
        {
            BirthDate = "02.01.1990",
            BirthDateDateEnd = "02.01.1992"
        };

        public static LeadSearchInputModel searchByEmailOperation = new LeadSearchInputModel()
        {
            Email = "%orot%",
            EmailOperator = 3
        };

        public static LeadSearchInputModel searchByPhoneOperation = new LeadSearchInputModel()
        {
            Phone = "%4102887%",
            PhoneOperator = 3
        };

        public static LeadSearchInputModel searchByLoginOperation = new LeadSearchInputModel()
        {
            Login = "%orot%",
            LoginOperator = 3
        };

        public static LeadSearchInputModel searchByRegistrationDateEnd = new LeadSearchInputModel()
        {
            RegistrationDate = "11.04.2020",
            RegistrationDateEnd = DateTime.Now.ToString()
        };

        public static LeadSearchInputModel searchByCityValues = new LeadSearchInputModel()
        {
            CitiesValues = "3, 2, 1"
        };

        public static LeadSearchInputModel searchByIsDeletedInclude = new LeadSearchInputModel()
        {
            IsDeletedInclude = true
        };

        public static LeadSearchInputModel searchByCombination = new LeadSearchInputModel()
        {
            FirstName = "%лия",
            FirstNameOperator = 3,
            LastName = "%орот%",
            LastNameOperator = 3,
            Patronymic = "%ьевна",
            PatronymicOperator = 3,
            BirthDate = "01.01.1990",
            BirthDateDateEnd = "03.01.1992",
            Email = "%orot%",
            EmailOperator = 3,
            Phone = "%4102887%",
            PhoneOperator = 3,
            Login = "%orot%",
            LoginOperator = 3,
            RegistrationDate = "11.04.2020",
            RegistrationDateEnd = DateTime.Now.ToString(),
            CitiesValues = "3, 2, 1",
            IsDeletedInclude = true,
            AccountId = 0
        };
    }
}
