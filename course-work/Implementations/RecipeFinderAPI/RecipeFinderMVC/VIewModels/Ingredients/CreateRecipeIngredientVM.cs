using System.ComponentModel.DataAnnotations;

namespace RecipeFinderMVC.VIewModels.Ingredients
{
    public class CreateRecipeIngredientVM
    {
        public string IngredientId { get; set; }

        public double Quantity { get; set; }

    }
}
