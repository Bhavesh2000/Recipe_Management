using System.ComponentModel.DataAnnotations;

namespace Recipe_Management_Frontend.Models
{
    public class Register
    {
        [Required(ErrorMessage ="* Name Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "* Not an valid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password Required")]
        public string Password { get; set; }
        [Compare("Password",ErrorMessage ="Password did not match")]
        public string ConfirmPassword { get; set; }

    }
}
