﻿//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Recipe_Management_System.AppDbContext;
//using Recipe_Management_System.Models;
//using Recipe_Management_System.Models.Dto;
//using Recipe_Management_System.Repository.Service;

//namespace Recipe_Management_System.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class RecipeController : ControllerBase
//    {
//        private readonly IRecipeService service;
//        private readonly IUserService uservice;

//        public RecipeController(IRecipeService _service, IUserService _uservice)
//        {
//            service = _service;
//            uservice = _uservice;
//        }

//        [HttpGet]
//        [Route("GetAllRecipes")]
//        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetAllRecipes()
//        {
//            var recipes = service.GetAllAsync();
//            var result = new List<RecipeDto>();
//            foreach (var recipe in recipes)
//            {
//                var users = uservice.GetAllAsync();
//                result.Add(new RecipeDto()
//                {
//                    Ingredients = recipe.Ingredients,
//                    name = recipe.Name,
//                    Procedure = recipe.Procedure,
//                    Status = recipe.Status,
//                    Username = users.FirstOrDefault(n => n.Id == recipe.UserId).Name
//                });
//            }

//            return result;
//        }

//        [HttpGet]
//        [Route("GetRecipeById")]
//        public async Task<ActionResult<RecipeDto>> GetRecipeById(int id)
//        {
//            var recipe = await service.GetByIdAsync(id);
//            var users = uservice.GetAllAsync();
//            RecipeDto result = new RecipeDto()
//            {
//                Ingredients = recipe.Value.Ingredients,
//                Procedure = recipe.Value.Procedure,
//                name = users.FirstOrDefault(n => n.Id == recipe.Value.UserId).Name,
//            };

//            if (result == null)
//            {
//                return NotFound();
//            }

//            return result;
//        }

//        [HttpGet]
//        [Route("GetRecipeByUserId")]
//        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetRecipeByUserId(int id)
//        {
//            var recipes = await service.GetRecipeByUserId(id);
//            var users = uservice.GetAllAsync();
//            List<RecipeDto> result = new List<RecipeDto>();
//            foreach (var recipe in recipes.Value)
//            {
//                result.Add(new RecipeDto()
//                {
//                    Ingredients = recipe.Ingredients,
//                    Procedure = recipe.Procedure,
//                    name = users.FirstOrDefault(n => n.Id == recipe.UserId).Name,
//                });
//            }

//            if (result == null)
//            {
//                return NotFound();
//            }

//            return result;
//        }

//        [HttpGet]
//        [Route("GetPendingRecipes")]
//        public async Task<ActionResult<IEnumerable<RecipeDto>>> GetPendingRecipes()
//        {
//            var recipes = await service.GetPendingRecipes();
//            var users = uservice.GetAllAsync();
//            List<RecipeDto> result = new List<RecipeDto>();
//            foreach (var recipe in recipes.Value)
//            {
//                result.Add(new RecipeDto()
//                {
//                    Ingredients = recipe.Ingredients,
//                    Procedure = recipe.Procedure,
//                    name = users.FirstOrDefault(n => n.Id == recipe.UserId).Name,
//                });
//            }

//            if (result == null)
//            {
//                return NotFound();
//            }

//            return result;
//        }



//        [HttpPost]
//        [Route("AddRecipe")]
//        public async Task AddRecipe(RecipeDto recipeDto)
//        {
//            var user = uservice.GetAllAsync().FirstOrDefault(n => n.Name == recipeDto.Username);
//            Recipe recipe = new Recipe()
//            {
//                Name = recipeDto.name,
//                Ingredients = recipeDto.Ingredients,
//                Procedure = recipeDto.Procedure,
//                Status = recipeDto.Status,
//                UserId = user.Id
//            };
//            await service.AddAsync(recipe);
//        }
//    }
//}
