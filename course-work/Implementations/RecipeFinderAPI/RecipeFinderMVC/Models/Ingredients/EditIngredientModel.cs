using System.ComponentModel.DataAnnotations;

namespace RecipeFinderMVC.Models.Ingredients
{
    public class EditIngredientModel
    {

        public string Id { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Ingredient name must be at least 3 characters long")]
        public string Name { get; set; }

        public string? ImgUrl { get; set; } = "https://images.ctfassets.net/kugm9fp9ib18/3aHPaEUU9HKYSVj1CTng58/d6750b97344c1dc31bdd09312d74ea5b/menu-default-image_220606_web.png";

        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Calories per 100g must be a positive number")]
        public double CaloriesPer100g { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Unit must be at least 1 character long")]
        public string Unit { get; set; }
        public bool? IsAllergen { get; set; }
    }
}
