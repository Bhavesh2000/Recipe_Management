using System.ComponentModel;

namespace Recipe_Management_System.Models.Dto
{
    public class AcceptedDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ingredients { get; set; }
        public string Procedure { get; set; }
        [DefaultValue("Pending")]
        public string Status { get; set; }
        public string Username { get; set; }
        public string Category { get; set; }
        public string UserId { get; set; }

    }
}
