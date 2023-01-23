using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Recipe_Management_System.AppDbContext;
using Recipe_Management_System.Models;
using Recipe_Management_System.Models.Dto;

namespace Recipe_Management_System.Repository.Service
{
    public class RecipeService : IRecipeService
    {
        private readonly ApplicationDbContext context;
        public RecipeService(ApplicationDbContext _context)
        {
            context = _context;
        }

        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetRecipesByName(string recipeName)
        {
            if (recipeName == null)
            {
                return new BadRequestObjectResult("Recipe name is not provided");
            }
            List<RecipeDto> result = new List<RecipeDto>();

            var recipes = await context.Recipes.Where(r => 
                            r.Name.Replace(" ","").ToLower().Contains(recipeName.ToLower()) ||
                            r.Name.ToLower().Contains(recipeName.ToLower()) &&
                            r.Status == "Accepted"
                            ).ToListAsync();
            if (recipes == null)
            {
                return result;
            }
            var users = context.Users.ToList();

            foreach (var recipe in recipes)
            {
                string Username = users.FirstOrDefault(n => n.Id == recipe.UserId).UserName;
                result.Add(new RecipeDto()
                {
                    Id = recipe.Id,
                    Ingredients = recipe.Ingredients,
                    Procedure = recipe.Procedure,
                    Username = Username,
                    name = recipe.Name,
                    Category = recipe.Category,
                    Status = recipe.Status,
                });
            }

            return result;
        }

        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetRecipesByUserName(string userName)
        {

            if (userName == null)
            {
                return new BadRequestObjectResult("UserName is not provided");
            }
            List<RecipeDto> result = new List<RecipeDto>();
            var users = context.Users.Where(x => x.UserName.ToLower().Contains(userName.ToLower()));
            if (users == null)
            {
                return result;
            }
            //List<Recipe> recipes=new List<Recipe>();
            foreach (var u in users)
            {
                var recipes = await context.Recipes.Where(r => r.UserId == u.Id && r.Status == "Accepted").ToListAsync();
                foreach (var recipe in recipes)
                {
                    result.Add(new RecipeDto()
                    {
                        Id = recipe.Id,
                        Ingredients = recipe.Ingredients,
                        Procedure = recipe.Procedure,
                        Username = u.UserName,
                        name = recipe.Name,
                        Category = recipe.Category,
                        Status = recipe.Status,
                    });
                }
            }

            return result;
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

        

        public IEnumerable<Recipe> GetAllAsync()
        {

            var actors = context.Set<Recipe>().ToList();
            return actors;

        }

        public async Task<ActionResult<IEnumerable<Recipe>>> GetAcceptedRecipes()
        {
            var recipes = await context.Recipes.Where(x => x.Status == "Accepted").ToListAsync();
            return recipes;
        }

        public async Task<ActionResult<Recipe>> GetByIdAsync(int id)
        {
            var actor = await context.Set<Recipe>().FirstOrDefaultAsync(n => n.Id == id);
            return actor;
        }

        public async Task<ActionResult<Recipe>> UpdateAsync(int id, Recipe addRecipeDto)
        {

            if (id != addRecipeDto.Id)
            {
                return new BadRequestObjectResult("Recipe not exists");
            }
           

            context.Entry(addRecipeDto).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return addRecipeDto;
        }

        public async Task<ActionResult<Recipe>> DeleteRecipe(int id)
        {
            var recipe = await context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return new BadRequestObjectResult("Not Found");
            }

            context.Recipes.Remove(recipe);
            await context.SaveChangesAsync();

            return recipe;
        }



        public async Task<ActionResult<Recipe>> Update_Status_Accept_Recipe(int id)
        {
            var recipe = await context.Recipes.FindAsync(id);
            
            if (recipe == null)
            {
                return new BadRequestObjectResult("Not Found");
            }
            recipe.Status = "Accepted";

            context.Entry(recipe).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return recipe;
        }

        public async Task<ActionResult<Recipe>> Update_Status_Reject_Recipe(int id)
        {
            var recipe = await context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return new BadRequestObjectResult("Not Found");
            }

            recipe.Status = "Rejected";

            context.Entry(recipe).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return recipe;
        }
    }
}
