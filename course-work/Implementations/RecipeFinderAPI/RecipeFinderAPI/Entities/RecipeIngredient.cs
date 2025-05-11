using System.ComponentModel.DataAnnotations;

namespace RecipeFinderAPI.Entities
{
    public class RecipeIngredient
    {
        public RecipeIngredient()
        {
            RecipeIngredientId = Guid.NewGuid().ToString(); 
        }
        public string RecipeIngredientId { get; set; }

        public string RecipeId {  get; set; }
        public string IngredientId { get; set;}

        [Range(0.001, double.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public double Quantity {  get; set; }

        public virtual Recipe Recipe { get; set; }
        public virtual Ingredient Ingredient {  get; set; }
    }
}
