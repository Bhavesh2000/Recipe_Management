using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recipe_Management_System.AppDbContext;
using Recipe_Management_System.Models;
using Recipe_Management_System.Repository.Base;

namespace Recipe_Management_System.Repository.Service
{
    public class RecipeService: EntityBaseRepository<Recipe>, IRecipeService
    {
        private readonly ApplicationDbContext context;
        public RecipeService(ApplicationDbContext _context) : base(_context)
        {
            context = _context;
        }

        
        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipeByUserId(int id)
        {
            var recipes=await context.Recipes.Where(x => x.UserId == id).ToListAsync();
            return recipes;
        }

        public async Task<ActionResult<IEnumerable<Recipe>>> GetPendingRecipes()
        {
            var recipes = await context.Recipes.Where(x => x.Status==false).ToListAsync();
            return recipes;
        }
    }
}
