using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recipe_Management_System.Models
{
    public class User : IdentityUser
    {
        [Required]
        public string Name { get; set; }
       
    }
}
