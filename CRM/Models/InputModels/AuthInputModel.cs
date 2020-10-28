using System.ComponentModel.DataAnnotations;

namespace CRM.API.Models.InputModels
{
    public class AuthInputModel
    {
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
