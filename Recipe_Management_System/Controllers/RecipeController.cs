using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recipe_Management_System.AppDbContext;
using Recipe_Management_System.Models;
using Recipe_Management_System.Models.Dto;
using Recipe_Management_System.Repository.Service;
using System.Security.Claims;

namespace Recipe_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService service;  
        private readonly IUserService uservice;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RecipeController(IRecipeService _service, IUserService _uservice, IHttpContextAccessor httpContextAccessor)
        {
            service = _service;
            uservice = _uservice;
            _httpContextAccessor = httpContextAccessor;
        }

        

        [HttpGet]
        [Route("GetAllRecipes")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        //Method to get all recipes from DB.
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
                // if result contains null value it will return not found error message
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
        [Route("Search")]
        //Method for search. It will get searched recipes or if searched by user will get user's recipes
        public async Task<object> Search(string search)
        {
            try
            {
                // Here it will return error messege if the parameter search contains null value
                if (search == null)
                {
                    return BadRequest("Type to Search");
                }
                List<RecipeDto> result = new List<RecipeDto>();
                var resultInRecipe = await service.GetRecipesByName(search);
                //Here it will add recipe by Recipe name to the list(result) if the following condition satisfies
                if (resultInRecipe != null)
                {

                    result.AddRange(resultInRecipe.Value);
                }
                var resultUsingUserName = await service.GetRecipesByUserName(search);
                //Here it will add recipe by user name to the list(result) if the following condition satisfies
                if (resultUsingUserName != null)
                {
                    result.AddRange(resultUsingUserName.Value);
                }
                // here it will return not found error messege if the recipe objects in the result list is zero
                if (result.Count() == 0)
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
        //Method to get recipe by id
        public async Task<ActionResult<RecipeDto>> GetRecipeById(int id)
        {

            try
            {
                // Here it will return error messege if the parameter id is not provided
                if (id == 0)
                {
                    return BadRequest("Id is not provided");
                }

                var recipe = await service.GetByIdAsync(id);
                // here it will return not found error messege if recipe is not exists for the provided Recipe Id
                if (recipe.Value == null)
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
        [Route("GetRecipeByUserId")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        //Method to get recipe related to User
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetRecipeByUserId()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
                // Here it will return error messege if UserId is null
                if (userId == null)
                {
                    return NotFound();
                }
                var user = uservice.GetAllAsync().FirstOrDefault(n => n.Id == userId);
                // here it will return not found error messege if user for the userId is not exists
                if (user == null)
                {
                    return NotFound();
                }
                var recipes = await service.GetRecipeByUserId(userId);
                // here it will return not found error messege if recipes for the UserId is not exists
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
                        Username = user.UserName,
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
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        //Method to get pending recipes which need to be approved or rejected. It is privileged only for Admin.
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetPendingRecipes()
        {
            try
            {
                var recipes = await service.GetPendingRecipes();
                // here it will return not found error messege if no recipes are in pending state
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
        [Authorize(AuthenticationSchemes = "Bearer")]
        //Method to add new recipe
        public async Task<ActionResult<AddRecipeDto>> AddRecipe(AddRecipeDto recipeDto)
        {
            var userId = _httpContextAccessor.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            // Here it will return error messege if UserId is null
            if (userId == null)
            {
                return NotFound();
            }
            // Here it will return error messege if ingredients is null in recipeDto object
            if (recipeDto.Ingredients == null)
            {
                return BadRequest("Please add Ingredients");
            }
            var user = uservice.GetAllAsync().FirstOrDefault(n => n.Id == userId);
            // here it will return not found error messege if user not exists for userId provided in recipeDto object
            if (user == null)
            {
                return BadRequest("User doesn't exists");
            }
            var recipeDuplicateName = service.GetAllAsync().Where(n => n.UserId == userId).ToList();
            // Here it will return error messege if recipe already exist for the perticular user
            if (recipeDuplicateName.Exists(n => n.Name.ToUpper() == recipeDto.name.ToUpper()))
            {
                return BadRequest("Recipe already exists");
            }

            Recipe recipe = new Recipe()
            {
                Name = recipeDto.name,
                Ingredients = recipeDto.Ingredients,
                Procedure = recipeDto.Procedure,
                UserId = userId,
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


        [HttpPut]
        [Route("Update_Status_Accept_Recipe")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        //Method to approve recipe posted by User. It is privileged only for Admin.
        public async Task<ActionResult<RecipeDto>> Update_Status_Accept_Recipe(int id)
        {
            // Here it will return error messege if the parameter id is not provided
            if (id == 0)
            {
                return BadRequest("Id is not provided");
            }

            var recipe = await service.Update_Status_Accept_Recipe(id);
            // here it will return not found error messege if recipe is not exists for the provided recipe Id 
            if (recipe.Value == null)
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

            return Ok(result);


        }


        [HttpPut]
        [Route("Update_Status_Reject_Recipe")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin")]
        //Method to reject recipe posted by User. It is privileged only for Admin.
        public async Task<ActionResult<RecipeDto>> Update_Status_Reject_Recipe(int id)
        {
            // Here it will return error messege if the parameter id is not provided
            if (id == 0)
            {
                return BadRequest("Id is not provided");
            }

            var recipe = await service.Update_Status_Reject_Recipe(id);
            // here it will return not found error messege if recipe is not exists for the provided recipe Id 
            if (recipe.Value == null)
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

            return Ok(result);

        }


        [HttpGet]
        [Route("GetAcceptedRecipes")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        //Method to get all approved recipes
        public async Task<ActionResult<IEnumerable<AcceptedDto>>> GetAcceptedRecipes()
        {
            try
            {
                var recipes = await service.GetAcceptedRecipes();
                // Here it will return not found error messege if no recipes are in accepted state
                if (recipes == null)
                {
                    return NotFound();
                }
                var users = uservice.GetAllAsync();
                List<AcceptedDto> result = new List<AcceptedDto>();
                foreach (var recipe in recipes.Value)
                {
                    result.Add(new AcceptedDto()
                    {
                        Id = recipe.Id,
                        Ingredients = recipe.Ingredients,
                        Procedure = recipe.Procedure,
                        Username = users.FirstOrDefault(n => n.Id == recipe.UserId).UserName,
                        Name = recipe.Name,
                        Category = recipe.Category,
                        Status = recipe.Status,
                        UserId = recipe.UserId
                    });
                }



                return result;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete]
        [Route("DeleteRecipe")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "User")]
        //Method to delete the recipe by User who posted it.
        public async Task<ActionResult<Recipe>> DeleteRecipe(int id)
        {
            // Here it will return error messege if id is not provided
            if (id == 0)
            {
                return new BadRequestObjectResult("Id not provided");
            }

            var recipe = await service.DeleteRecipe(id);

            return recipe;
        }


        [HttpPut]
        [Route("UpdateRecipe")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "User")]
        //Method to update the recipe by User who posted it. The status of updated recipe will set to pending by default.
        public async Task<ActionResult<Recipe>> UpdateRecipe(UpdateDto addRecipeDto)
        {
            // Here it will return error messege if updateRecipe parameter is not provided
            if (addRecipeDto == null)
            {
                return BadRequest("Required fields are not provided");
            }

            var userId = _httpContextAccessor.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            // Here it will return error messege if UserId is null
            if (userId == null)
            {
                return NotFound();
            }
            // Here it will return error messege if ingredients is null in recipeDto object
            if (addRecipeDto.Ingredients == null)
            {
                return BadRequest("Please provide Ingredients");
            }
            var recipe = new Recipe()
            {
                Id = addRecipeDto.Id,
                Name = addRecipeDto.name,
                Ingredients = addRecipeDto.Ingredients,
                Procedure = addRecipeDto.Procedure,
                UserId = userId,
                Category = addRecipeDto.Category,
                Status = "Pending",
            };

            try
            {
                

                return await service.UpdateAsync(addRecipeDto.Id, recipe);

            }
            catch (Exception )
            {
                return BadRequest("Internal server error");
            }

            


        }



    }
}
