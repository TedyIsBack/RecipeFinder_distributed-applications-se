using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RecipeFinderAPI.Entities
{
    public class Recipe
    {
        public Recipe()
        {
            RecipeId = Guid.NewGuid().ToString();
        }

        public string RecipeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public string ImgUrl { get; set; }

        [Required]
        [Range(1, 720)]
        public int PreparationTime { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double Calories { get; set; }

        public bool IsVegan { get; set; }

        public bool IsVegetarian { get; set; }

        [Required]
        public string CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<RecipeIngredient> RecipeIngredients { get; set; }
        public virtual ICollection<FavoriteRecipe> FavoriteRecipes { get; set; }

    }

}
