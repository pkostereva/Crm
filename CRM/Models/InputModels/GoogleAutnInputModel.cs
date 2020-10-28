using System.ComponentModel.DataAnnotations;

namespace CRM.API.Models.InputModels.AuthInputModels
{
    public class GoogleAutnInputModel
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
