namespace Recipe_Management_System.Models
{
    public enum Category{
        Veg,
        Non_Veg
    }
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ingredients { get; set; }
        public string Procedure { get; set; }
        //Foreign Key from User
        public int UserId { get; set; }
        //Status is to checks the recipe and approve it by admin.
        public bool Status { get; set; }

    }
}
