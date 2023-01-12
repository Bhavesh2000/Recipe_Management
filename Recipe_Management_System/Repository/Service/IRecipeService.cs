using Microsoft.AspNetCore.Mvc;
using Recipe_Management_System.Models;
using Recipe_Management_System.Repository.Base;

namespace Recipe_Management_System.Repository.Service
{
    public interface IRecipeService : IEntityBaseRepository<Recipe>
    {
        Task<ActionResult<IEnumerable<Recipe>>> GetRecipeByUserId(int id);
        Task<ActionResult<IEnumerable<Recipe>>> GetPendingRecipes();
    }
}
