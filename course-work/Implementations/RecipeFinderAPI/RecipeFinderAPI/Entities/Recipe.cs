using System.Collections.Generic;

namespace RecipeFinderAPI.Entities
{
    public class Recipe
    {
        public Recipe()
        {
            RecipeId = Guid.NewGuid().ToString();
        }

        public string RecipeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PreparationTime { get; set; }
        public double Calories { get; set; }
        public string? Difficulty { get; set; }
        public bool IsVegan { get; set; }
        public bool IsVegetarian { get; set; }

        public string CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; }
        public virtual ICollection<FavoriteRecipe> FavoriteRecipes { get; set; }

    }

}
