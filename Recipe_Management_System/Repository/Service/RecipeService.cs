using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Recipe_Management_System.AppDbContext;
using Recipe_Management_System.Models;


namespace Recipe_Management_System.Repository.Service
{
    public class RecipeService : IRecipeService
    {
        private readonly ApplicationDbContext context;
        public RecipeService(ApplicationDbContext _context)
        {
            context = _context;
        }


        public async Task<ActionResult<IEnumerable<Recipe>>> GetRecipeByUserId(string id)
        {
            var recipes = await context.Recipes.Where(x => x.UserId == id).ToListAsync();
            return recipes;
        }

        public async Task<ActionResult<IEnumerable<Recipe>>> GetPendingRecipes()
        {
            var recipes = await context.Recipes.Where(x => x.Status == "Pending").ToListAsync();
            return recipes;
        }

        public async Task<ActionResult<Recipe>> AddAsync(Recipe entity)
        {
            await context.Set<Recipe>().AddAsync(entity);
            await context.SaveChangesAsync();
            return entity;

        }

        public async Task DeleteAsync(int id)
        {
            var entity = await context.Set<Recipe>().FirstOrDefaultAsync(n => n.Id == id);
            EntityEntry entityEntry = context.Entry<Recipe>(entity);
            entityEntry.State = EntityState.Deleted;
            context.SaveChanges();
        }

        public IEnumerable<Recipe> GetAllAsync()
        {

            var actors = context.Set<Recipe>().ToList();
            return actors;



        }

        public async Task<ActionResult<Recipe>> GetByIdAsync(int id)
        {
            var actor = await context.Set<Recipe>().FirstOrDefaultAsync(n => n.Id == id);
            return actor;
        }

        public async Task UpdateAsync(int id, Recipe entity)
        {
            EntityEntry entityEntry = context.Entry<Recipe>(entity);
            entityEntry.State = EntityState.Modified;
            context.SaveChanges();
        }
    }
}
