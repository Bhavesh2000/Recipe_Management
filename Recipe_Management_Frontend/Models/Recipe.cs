﻿namespace Recipe_Management_Frontend.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Ingredients { get; set; }
        public string Procedure { get; set; }
        public string Status { get; set; }
        public string Username { get; set; }
        public string Category { get; set; }
    }
}
