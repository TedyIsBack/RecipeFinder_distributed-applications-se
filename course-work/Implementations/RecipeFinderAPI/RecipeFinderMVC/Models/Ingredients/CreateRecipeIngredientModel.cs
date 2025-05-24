using System.ComponentModel.DataAnnotations;

namespace RecipeFinderMVC.Models.Ingredients
{
    public class CreateRecipeIngredientModel
    {
        public string IngredientId { get; set; }

        public double Quantity { get; set; }

    }
}
