using Microsoft.EntityFrameworkCore;
using Recipe_Management_System.AppDbContext;
using Recipe_Management_System.Repository.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IRecipeService, RecipeService>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();

//Adding service so that DbContext will have info of server and database to be connected.
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
