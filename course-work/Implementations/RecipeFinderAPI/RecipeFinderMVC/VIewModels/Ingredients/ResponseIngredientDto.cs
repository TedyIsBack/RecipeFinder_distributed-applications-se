using System.ComponentModel.DataAnnotations;

namespace RecipeFinderAPI.Infrastructure.DTOs.IngredientDTOs
{
    public class ResponseIngredientDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImgUrl { get; set; }
        public double CaloriesPer100g { get; set; }
        public string Unit { get; set; }
        public bool? IsAllergen { get; set; }
    }
}
