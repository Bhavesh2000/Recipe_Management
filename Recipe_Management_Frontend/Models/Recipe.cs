using System.ComponentModel;

namespace Recipe_Management_Frontend.Models
{
    public class Recipe
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
