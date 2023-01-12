using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Recipe_Management_System.Models.Dto
{
    public class RecipeDto
    {
        public string name { get; set; }
        public string Ingredients { get; set; }
        public string Procedure { get; set; }
        [DefaultValue("Pending")]
        public string Status { get; set; }
        public string Username { get; set; }
        public string Category { get; set; }
        
    }
}
