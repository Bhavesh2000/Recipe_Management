using Microsoft.AspNetCore.Mvc;
using Recipe_Management_System.Models;

namespace Recipe_Management_System.Repository.Service
{
    public interface IRecipeService
    {
        Task<ActionResult<IEnumerable<Recipe>>> GetRecipeByUserId(string id);
        Task<ActionResult<IEnumerable<Recipe>>> GetPendingRecipes();
        Task<ActionResult<Recipe>> GetByIdAsync(int id);
        Task<ActionResult<Recipe>> AddAsync(Recipe entity);
        Task UpdateAsync(int id, Recipe entity);
        Task DeleteAsync(int id);
        IEnumerable<Recipe> GetAllAsync();
    }
}
