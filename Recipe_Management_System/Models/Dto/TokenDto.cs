using System.ComponentModel.DataAnnotations;

namespace Recipe_Management_System.Models.Dto
{
    public class TokenDto
    {
        [Required]
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
