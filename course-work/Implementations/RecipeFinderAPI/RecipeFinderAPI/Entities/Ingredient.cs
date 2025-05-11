using System.ComponentModel.DataAnnotations;

namespace RecipeFinderAPI.Entities
{
    public class Ingredient
    {
        public Ingredient()
        {
            IngredientId = new Guid().ToString();
        }
        public string IngredientId { get; set; }


        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public string ImgUrl { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public double CaloriesPer100g { get; set; }

        [Required]
        [MaxLength(20)]
        public string Unit { get; set; }
        public bool? IsAllergen { get; set; }
        public virtual ICollection<RecipeIngredient> RecipeIngredients {  get; set; }
    }
}
