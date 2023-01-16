using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Recipe_Management_System.Models.Dto
{
    public class RegisterDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        //Type is to identify normal User & Admin
        //[DefaultValue("User")]
        //public string Type { get; set; }
    }
}
