using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recipe_Management_System.AppDbContext;
using Recipe_Management_System.Models;
using Recipe_Management_System.Models.Dto;
using Recipe_Management_System.Repository.Service;

namespace Recipe_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService service;
        private readonly IUserService uservice;

        public RecipeController(IRecipeService _service, IUserService _uservice)
        {
            service = _service;
            uservice = _uservice;
        }

        [HttpGet]
        [Route("GetAllRecipes")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetAllRecipes()
        {
            try
            {
                var recipes = service.GetAllAsync();
                var result = new List<RecipeDto>();
                foreach (var recipe in recipes)
                {
                    var users = uservice.GetAllAsync();
                    result.Add(new RecipeDto()
                    {
                        Id = recipe.Id,
                        Ingredients = recipe.Ingredients,
                        name = recipe.Name,
                        Procedure = recipe.Procedure,
                        Username = users.FirstOrDefault(n => n.Id == recipe.UserId).UserName,
                        Category = recipe.Category,
                        Status = recipe.Status,
                    });
                }

                if (result == null)
                {
                    return NotFound();
                }

                return result;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        [Route("GetRecipeById")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<RecipeDto>> GetRecipeById(int id)
        {

            try
            {
                if (id == 0)
                {
                    return BadRequest("Id is not provided");
                }

                var recipe = await service.GetByIdAsync(id);
                if (recipe.Value==null)
                {
                    return NotFound();
                }
                var users = uservice.GetAllAsync();
                RecipeDto result = new RecipeDto()
                {
                    Id = id,
                    Ingredients = recipe.Value.Ingredients,
                    Procedure = recipe.Value.Procedure,
                    Username = users.FirstOrDefault(n => n.Id == recipe.Value.UserId).UserName,
                    name = recipe.Value.Name,
                    Category = recipe.Value.Category,
                    Status = recipe.Value.Status,
                };
                
                return result;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet]
        [Route("GetRecipeByUserName")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetRecipeByUserName(string Username)
        {
            try
            {
                if (Username == null)
                {
                    return BadRequest("UserName is not provided");
                }
                var user = uservice.GetAllAsync().FirstOrDefault(n=>n.UserName == Username);
                if (user == null)
                {
                    return NotFound();
                }
                var recipes = await service.GetRecipeByUserId(user.Id);
                if (recipes.Value == null)
                {
                    return NotFound();
                }
                List<RecipeDto> result = new List<RecipeDto>();
                foreach (var recipe in recipes.Value)
                {
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


        }

        [HttpGet]
        [Route("GetPendingRecipes")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetPendingRecipes()
        {
            try
            {
                var recipes = await service.GetPendingRecipes();
                if (recipes == null)
                {
                    return NotFound();
                }
                var users = uservice.GetAllAsync();
                List<RecipeDto> result = new List<RecipeDto>();
                foreach (var recipe in recipes.Value)
                {
                    result.Add(new RecipeDto()
                    {
                        Id = recipe.Id,
                        Ingredients = recipe.Ingredients,
                        Procedure = recipe.Procedure,
                        Username = users.FirstOrDefault(n => n.Id == recipe.UserId).UserName,
                        name = recipe.Name,
                        Category = recipe.Category,
                        Status = recipe.Status,
                    });
                }

                

                return result;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



        [HttpPost]
        [Route("AddRecipe")]
        //[Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<AddRecipeDto>> AddRecipe(AddRecipeDto recipeDto)
        {
            var user = uservice.GetAllAsync().FirstOrDefault(n => n.UserName == recipeDto.Username);
            if (user == null)
            {
                return BadRequest("Required fields are not provided");
            }
            var recipeDuplicateName = service.GetAllAsync().Where(n=>n.UserId == user.Id).ToList();

            if(recipeDuplicateName.Exists(n=>n.Name.ToUpper() == recipeDto.name.ToUpper()))
            {
                return BadRequest("Recipe already exists");
            }

            Recipe recipe = new Recipe()
            {
                Name = recipeDto.name,
                Ingredients = recipeDto.Ingredients,
                Procedure = recipeDto.Procedure,
                UserId = user.Id,
                Category = recipeDto.Category,
                Status = recipeDto.Status,
            };
            try
            {


                await service.AddAsync(recipe);

            }
            catch (NullReferenceException ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(recipeDto);
        }

        //[HttpPut]
        //[Route("AcceptRecipe/{id:int}")]
        //public async Task<IActionResult> AcceptedRecipe(int recipeId)
        //{
        //    //id is present in our recipe table

        //    //
        //}

        //[HttpPut]
        //[Route("RejectRecipe/{id:int}")]
        //public async Task<IActionResult> RejectRecipe(int id)


        //Get recipes which are accpeted.
    }
}
