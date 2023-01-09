using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Recipe_Management_System.AppDbContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
    }
}
