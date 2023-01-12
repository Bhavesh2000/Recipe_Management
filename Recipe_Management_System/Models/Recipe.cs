using Recipe_Management_System.Repository.Base;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Recipe_Management_System.Models
{
    public enum Category{
        Veg,
        Non_Veg
    }
    public class Recipe : IEntityBase
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Ingredients { get; set; }
        [Required]
        public string Procedure { get; set; }
        //Foreign Key from User
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
        //Status is to checks the recipe and approve it by admin.
        public bool Status { get; set; }

    }
}
