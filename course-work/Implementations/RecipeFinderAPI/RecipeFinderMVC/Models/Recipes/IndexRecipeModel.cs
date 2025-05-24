using RecipeFinderAPI.Infrastructure.DTOs.AccountDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.CategoryDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.IngredientDTOs;
using RecipeFinderMVC.Models.Accounts;
using RecipeFinderMVC.Models.Categories;
using RecipeFinderMVC.Models.Ingredients;
using System.ComponentModel.DataAnnotations;

namespace RecipeFinderMVC.Models.Recipes
{
    public class IndexRecipeModel
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
        public IndexCategoryModel Category { get; set; }
        public string CreatedBy { get; set; }

        public bool IsFavorite { get; set; }
        public IndexAccountModel CreatedByUser { get; set; }
        public ICollection<IndexRecipeIngredientModel> RecipeIngredients { get; set; }


    }
}
