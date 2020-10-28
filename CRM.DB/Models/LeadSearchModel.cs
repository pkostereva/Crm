using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.DB.Models
{
    public class LeadSearchModel
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
        public DateTime? BirthDateDateEnd { get; set; }
        public string Phone { get; set; }
        public int? PhoneOperator { get; set; }
        public string Email { get; set; }
        public int? EmailOperator { get; set; }
        public string Login { get; set; }
        public int? LoginOperator { get; set; }
        public int? CityId { get; set; }
        public int? CityIdOperator { get; set; }
        public string CitiesValues { get; set; }
        public string RegistrationDate { get; set; }
        public int? RegistrationDateOperator { get; set; }
        public DateTime? RegistrationDateEnd { get; set; }
        public bool? IsDeletedInclude { get; set; }
        public long? AccountId { get; set; }
    }
}
