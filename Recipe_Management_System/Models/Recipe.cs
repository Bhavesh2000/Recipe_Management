
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recipe_Management_System.Models
{
    
    public class Recipe
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Category { get; set; }
        [Required]
        public string Ingredients { get; set; }
        [Required]
        public string Procedure { get; set; }
        //Foreign Key from User

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        //Status is to checks the recipe and approve it by admin.
        [DefaultValue("Pending")]
        public string Status { get; set; } 

    }
}
