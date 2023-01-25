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
        [Route("Search")]
        public async Task<object> Search(string search)
        {
            try
            {
                if (search == null)
                {
                    return BadRequest("Type to Search");
                }
                List<RecipeDto> result = new List<RecipeDto>();
                var resultInRecipe = await service.GetRecipesByName(search);
                if (resultInRecipe != null)
                {

                    result.AddRange(resultInRecipe.Value);
                }
                var resultUsingUserName = await service.GetRecipesByUserName(search);
                if (resultUsingUserName != null)
                {
                    result.AddRange(resultUsingUserName.Value);
                }
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
        public async Task<ActionResult<RecipeDto>> GetRecipeById(int id)
        {

            try
            {
                if (id == 0)
                {
                    return BadRequest("Id is not provided");
                }

                var recipe = await service.GetByIdAsync(id);
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
        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetRecipeByUserId()
        {
            try
            {
                var userId = _httpContextAccessor.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
                if(userId == null)
                {
                    return NotFound();
                }
                var user = uservice.GetAllAsync().FirstOrDefault(n => n.Id == userId);
                if (user == null)
                {
                    return NotFound();
                }
                var recipes = await service.GetRecipeByUserId(userId);
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
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult<AddRecipeDto>> AddRecipe(AddRecipeDto recipeDto)
        {
            var userId = _httpContextAccessor.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            if (userId == null)
            {
                return NotFound();
            }

            if (recipeDto.Ingredients == null)
            {
                return BadRequest("Please add Ingredients");
            }
            var user = uservice.GetAllAsync().FirstOrDefault(n => n.Id == userId);
            if (user == null)
            {
                return BadRequest("User doesn't exists");
            }
            var recipeDuplicateName = service.GetAllAsync().Where(n => n.UserId == userId).ToList();

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
        public async Task<ActionResult<RecipeDto>> Update_Status_Accept_Recipe(int id)
        {

            if (id == 0)
            {
                return BadRequest("Id is not provided");
            }

            var recipe = await service.Update_Status_Accept_Recipe(id);
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
        public async Task<ActionResult<RecipeDto>> Update_Status_Reject_Recipe(int id)
        {
            if (id == 0)
            {
                return BadRequest("Id is not provided");
            }

            var recipe = await service.Update_Status_Reject_Recipe(id);
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
        public async Task<ActionResult<IEnumerable<AcceptedDto>>> GetAcceptedRecipes()
        {
            try
            {
                var recipes = await service.GetAcceptedRecipes();
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
        public async Task<ActionResult<Recipe>> DeleteRecipe(int id)
        {

            if (id == null)
            {
                return new BadRequestObjectResult("Id not provided");
            }

            var recipe = await service.DeleteRecipe(id);

            return recipe;
        }


        [HttpPut]
        [Route("UpdateRecipe")]
        [Authorize(AuthenticationSchemes = "Bearer", Roles = "User")]
        public async Task<ActionResult<Recipe>> UpdateRecipe(UpdateDto addRecipeDto)
        {
            if (addRecipeDto == null)
            {
                return BadRequest("Required fields are not provided");
            }

            var userId = _httpContextAccessor.HttpContext.User.Claims.First(i => i.Type == "Id").Value;
            if (userId == null)
            {
                return NotFound();
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
                //var recipeDuplicateName = service.GetAllAsync().Where(n => n.UserId == addRecipeDto.UserId).ToList();

                //if (recipeDuplicateName.Exists(n => n.Name.ToUpper() == addRecipeDto.name.ToUpper()))
                //{
                //    return BadRequest("Recipe already exists");
                //}

                return await service.UpdateAsync(addRecipeDto.Id, recipe);

            }
            catch (Exception )
            {
                return BadRequest("Internal server error");
            }

            


        }



    }
}
