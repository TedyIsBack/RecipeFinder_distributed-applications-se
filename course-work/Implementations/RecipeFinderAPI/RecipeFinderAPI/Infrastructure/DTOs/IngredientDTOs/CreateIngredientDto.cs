using System.ComponentModel.DataAnnotations;

namespace RecipeFinderAPI.Infrastructure.DTOs.IngredientDTOs
{
    public class CreateIngredientDto
    {
        [Required]
        [StringLength(50,MinimumLength = 3,ErrorMessage = "Ingredient name must be at least 3 characters long")]
        public string Name { get; set; }

        //TODO: Set default value if not provided - https://images.ctfassets.net/kugm9fp9ib18/3aHPaEUU9HKYSVj1CTng58/d6750b97344c1dc31bdd09312d74ea5b/menu-default-image_220606_web.png
        public string ImgUrl { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Calories per 100g must be a positive number")]
        public double CaloriesPer100g { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Unit must be at least 1 character long")]
        public string Unit { get; set; }
        public bool? IsAllergen { get; set; }
    }
}
