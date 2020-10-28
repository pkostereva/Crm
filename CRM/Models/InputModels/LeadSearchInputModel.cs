using System;
using System.ComponentModel;

namespace CRM.API.Models.InputModels
{
    public class LeadSearchInputModel : ICloneable
    {
        public long? Id { get; set; }
        public int? IdOperator { get; set; }
        public int? IdEnd { get; set; }
        public string FirstName { get; set; }
        public int? FirstNameOperator { get; set; }
        public string LastName { get; set; }
        public int? LastNameOperator { get; set; }
        public string Patronymic { get; set; }
        public int? PatronymicOperator { get; set; }
        public string BirthDate { get; set; }
        public int? BirthDateOperator { get; set; }
        public string BirthDateDateEnd { get; set; }
        public string Phone { get; set; }
        public int? PhoneOperator { get; set; }
        public string Email { get; set; }
        public int? EmailOperator { get; set; }
        public string Login { get; set; }
        public int? LoginOperator { get; set; }
        public int? CityId { get; set; }
        public string CitiesValues { get; set; }
        public string RegistrationDate { get; set; }
        public int? RegistrationDateOperator { get; set; }
        public string RegistrationDateEnd { get; set; }
        [DefaultValue(false)]
        public bool IsDeletedInclude { get; set; }
        public long AccountId { get; set; }

        public object Clone()
        {
            return new LeadSearchInputModel()
            {
                Id = Id,
                IdOperator = IdOperator,
                IdEnd = IdEnd,
                FirstName = FirstName,
                FirstNameOperator = FirstNameOperator,
                LastName = LastName,
                LastNameOperator = LastNameOperator,
                Patronymic = Patronymic,
                PatronymicOperator = PatronymicOperator,
                BirthDate = BirthDate,
                BirthDateOperator = BirthDateOperator,
                BirthDateDateEnd = BirthDateDateEnd,
                Phone = Phone,
                PhoneOperator = PhoneOperator,
                Email = Email,
                EmailOperator = EmailOperator,
                Login = Login,
                LoginOperator = LoginOperator,
                CityId = CityId,
              //  CityIdOperator = CityIdOperator,
                CitiesValues = CitiesValues,
                RegistrationDate = RegistrationDate,
                RegistrationDateOperator = RegistrationDateOperator,
                RegistrationDateEnd = RegistrationDateEnd,
                IsDeletedInclude = IsDeletedInclude,
                AccountId = AccountId
            };
        }
    }
}