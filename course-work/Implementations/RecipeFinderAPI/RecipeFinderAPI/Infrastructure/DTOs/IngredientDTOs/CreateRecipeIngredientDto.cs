using System.ComponentModel.DataAnnotations;

namespace RecipeFinderAPI.Infrastructure.DTOs.IngredientDTOs
{
    public class CreateRecipeIngredientDto
    {
        public string IngredientId { get; set; }

        public double Quantity { get; set; }

    }
}
