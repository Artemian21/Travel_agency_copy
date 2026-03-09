using System.ComponentModel.DataAnnotations;

namespace Travel_agency.Core.BusinessModels.Users
{
    public class RegisterUserModel
    {
        public string Username { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
