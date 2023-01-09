namespace Recipe_Management_System.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        //Type is to identify normal User & Admin
        public string Type { get; set; }
    }
}
