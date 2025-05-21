using RecipeFinderAPI.Infrastructure.DTOs.CategoryDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.IngredientDTOs;
using RecipeFinderMVC.VIewModels.Ingredients;
using System.ComponentModel.DataAnnotations;

namespace RecipeFinderMVC.VIewModels.Recipes
{
    public class CreateRecipeVM
    {
        [Required]
        [StringLength(100, MinimumLength = 10, ErrorMessage ="Recipe name must be at least 10 characters long.")]
        [Display(Name = "Recipe Name")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }


        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }

        public string ImgUrl { get; set; } = "https://images.ctfassets.net/kugm9fp9ib18/3aHPaEUU9HKYSVj1CTng58/d6750b97344c1dc31bdd09312d74ea5b/menu-default-image_220606_web.png";


        [Required]
        [Range(10, int.MaxValue, ErrorMessage = "Preparation time must be at least 10 minutes.")]
        [Display(Name = "Preparation Time (in minutes)")]
        public int PreparationTime { get; set; }

        //TODO: Calories will be calculated based on the ingredients
        //public double Calories { get; set; }

        [Required]
        [Display(Name = "Vegan")]
        public bool IsVegan { get; set; }

        [Required]
        [Display(Name = "Vegetarian")]
        public bool IsVegetarian { get; set; }

        [Required]
        public string CategoryId { get; set; }

        [Required]
        public ICollection<CreateRecipeIngredientVM> RecipeIngredients { get; set; }
    }
}
