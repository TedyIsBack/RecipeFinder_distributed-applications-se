﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using RecipeFinderMVC.Models.Categories;
using RecipeFinderMVC.Models.Ingredients;
using System.ComponentModel.DataAnnotations;

namespace RecipeFinderMVC.Models.Recipes
{
    public class EditRecipeModel
    {

        public string Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 10, ErrorMessage = "Recipe name must be at least 10 characters long.")]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }
        public string ImgUrl { get; set; } = "https://images.ctfassets.net/kugm9fp9ib18/3aHPaEUU9HKYSVj1CTng58/d6750b97344c1dc31bdd09312d74ea5b/menu-default-image_220606_web.png";
        [Required]
        [Range(10, int.MaxValue, ErrorMessage = "Preparation time must be at least 10 minutes.")]
        public int PreparationTime { get; set; }

        //TODO: Calories will be calculated based on the ingredients
        //public double Calories { get; set; }

        [Required]
        public bool IsVegan { get; set; }

        [Required]
        public bool IsVegetarian { get; set; }

        [Required]
        public string CategoryId { get; set; }

        // [Required]
        // public ResponseCategoryDto Category { get; set; }
        [Required]
        public ICollection<CreateRecipeIngredientModel> RecipeIngredients { get; set; }
        [BindNever]
        public IEnumerable<IndexCategoryModel> AvailableCategories { get; set; } = new List<IndexCategoryModel>();

        [BindNever]
        public IEnumerable<IndexIngredientModel> AvailableIngredients { get; set; } = new List<IndexIngredientModel>();
    }
}
