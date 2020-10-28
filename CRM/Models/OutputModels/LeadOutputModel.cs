using System;
using System.Collections.Generic;

namespace CRM.API.Models.OutputModels
{
	public class LeadOutputModel
	{
        public long? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string BirthDate { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Login { get; set; }
        public string CityName { get; set; }
        public string RegistrationDate { get; set; }
        public string LastUpdateDate { get; set; }
        public List<AccountOutputModel> Accounts { get; set; }

        public override bool Equals(object obj)
        {
            return obj is LeadOutputModel model &&
                   Id == model.Id &&
                   FirstName == model.FirstName &&
                   LastName == model.LastName &&
                   Patronymic == model.Patronymic &&
                   BirthDate == model.BirthDate &&
                   Email == model.Email &&
                   Phone == model.Phone &&
                   Login == model.Login &&
                   CityName == model.CityName &&
                   RegistrationDate == model.RegistrationDate &&
                   LastUpdateDate == model.LastUpdateDate;
        }

        public bool EqualsForLeadTest(object obj)
        {
            return obj is LeadOutputModel leadOutputModel &&
                   Id == leadOutputModel.Id &&
                   FirstName == leadOutputModel.FirstName &&
                   LastName == leadOutputModel.LastName &&
                   Patronymic == leadOutputModel.Patronymic &&
                   BirthDate == leadOutputModel.BirthDate &&
                   Email == leadOutputModel.Email &&
                   Phone == leadOutputModel.Phone &&
                   Login == leadOutputModel.Login;
        }

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            hash.Add(Id);
            hash.Add(FirstName);
            hash.Add(LastName);
            hash.Add(Patronymic);
            hash.Add(BirthDate);
            hash.Add(Email);
            hash.Add(Phone);
            hash.Add(Login);
            hash.Add(CityName);
            hash.Add(RegistrationDate);
            hash.Add(LastUpdateDate);
            return hash.ToHashCode();
        }
    }
}
