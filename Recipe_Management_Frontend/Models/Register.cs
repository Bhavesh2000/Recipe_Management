using System.ComponentModel.DataAnnotations;

namespace Recipe_Management_Frontend.Models
{
    public class Register
    {
        [Required(ErrorMessage ="* UserName Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "* Email Required")]
        [EmailAddress(ErrorMessage ="Enter valid email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "* Password Required ")]
        public string Password { get; set; }
        [Required(ErrorMessage = "* Confirm Password Required")]
        [Compare("Password",ErrorMessage ="Password did not match")]
        public string ConfirmPassword { get; set; }

    }
}
