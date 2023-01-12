using System.ComponentModel.DataAnnotations;

namespace Recipe_Management_System.Models.Dto
{
    public class LoginDto
    {
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
