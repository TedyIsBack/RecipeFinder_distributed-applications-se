using RecipeFinderAPI.Infrastructure.DTOs.CategoryDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.IngredientDTOs;
using System.ComponentModel.DataAnnotations;

namespace RecipeFinderAPI.Infrastructure.DTOs.RecipeDTOs
{
    public class ResponseRecipeDto
    {
      
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string ImgUrl { get; set; }

        public int PreparationTime { get; set; }

        public double Calories { get; set; }
        public bool IsVegan { get; set; }
        
        public bool IsVegetarian { get; set; }
        public string CategoryId { get; set; }
        public ResponseCategoryDto Category { get; set; }
        public ICollection<ResponseIngredientDto> RecipeIngredients { get; set; }
    }
}
