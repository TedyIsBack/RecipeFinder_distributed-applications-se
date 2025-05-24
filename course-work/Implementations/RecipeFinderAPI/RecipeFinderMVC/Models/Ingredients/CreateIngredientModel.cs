using System.ComponentModel.DataAnnotations;

namespace RecipeFinderMVC.Models.Ingredients
{
    public class CreateIngredientModel
    {
        [Required]
        [StringLength(50,MinimumLength = 3,ErrorMessage = "Ingredient name must be at least 3 characters long")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Image URL")]
        public string ImgUrl { get; set; } = "https://images.ctfassets.net/kugm9fp9ib18/3aHPaEUU9HKYSVj1CTng58/d6750b97344c1dc31bdd09312d74ea5b/menu-default-image_220606_web.png";

        [Display(Name = "Upload Image")]
        public IFormFile? ImageFile { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Calories per 100g must be a positive number")]
        [Display(Name = "Calories per 100g")]
        public double CaloriesPer100g { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Unit must be at least 1 character long")]
        [Display(Name = "Unit")]
        public string Unit { get; set; }

        [Display(Name = "Is Allergen")]
        public bool? IsAllergen { get; set; }
    }
}
