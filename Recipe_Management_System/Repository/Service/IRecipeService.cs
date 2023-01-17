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
        Task<ActionResult<Recipe>> UpdateAsync(int id, Recipe entity);
        Task<ActionResult<Recipe>> DeleteRecipe(int id);
        IEnumerable<Recipe> GetAllAsync();
        Task<ActionResult<Recipe>> Update_Status_Accept_Recipe(int id);
        Task<ActionResult<Recipe>> Update_Status_Reject_Recipe(int id);
        Task<ActionResult<IEnumerable<Recipe>>> GetAcceptedRecipes();
    }
}
