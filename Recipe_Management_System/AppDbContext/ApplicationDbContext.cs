﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Recipe_Management_System.Models;

namespace Recipe_Management_System.AppDbContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Recipe> Recipes { get; set; } 
    }
}
