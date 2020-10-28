using System;
using System.ComponentModel.DataAnnotations;

namespace CRM.API.Models.InputModels
{
	public class LeadInputModel : ICloneable
	{
        public long? Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        [Required]
        public string BirthDate { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public int CityId { get; set; }

        public object Clone()
        {
            return new LeadInputModel()
            {
                FirstName = FirstName,
                LastName = LastName,
                Patronymic = Patronymic,
                BirthDate = BirthDate,
                Email = Email,
                Phone = Phone,
                Login = Login,
                Password = Password,
                CityId = CityId
            };
        }

        public override bool Equals(object obj)
        {
            return obj is LeadInputModel leadInputModel &&
                   Id == leadInputModel.Id &&
                   FirstName == leadInputModel.FirstName &&
                   LastName == leadInputModel.LastName &&
                   Patronymic == leadInputModel.Patronymic &&
                   BirthDate == leadInputModel.BirthDate &&
                   Email == leadInputModel.Email &&
                   Phone == leadInputModel.Phone &&
                   Login == leadInputModel.Login &&
                   CityId == leadInputModel.CityId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(FirstName, LastName,
                Patronymic, BirthDate, Email, Phone, Login, CityId);
        }
    }
}
