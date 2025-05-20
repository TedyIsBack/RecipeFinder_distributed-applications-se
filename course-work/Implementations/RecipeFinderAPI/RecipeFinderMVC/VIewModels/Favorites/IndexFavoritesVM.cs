using RecipeFinderAPI.Infrastructure.DTOs.CategoryDTOs;
using RecipeFinderAPI.Infrastructure.DTOs.IngredientDTOs;
using RecipeFinderMVC.VIewModels.Categories;
using RecipeFinderMVC.VIewModels.Ingredients;

namespace RecipeFinderMVC.VIewModels.Favorites
{
    public class IndexFavoritesVM
    {

        //public string Id { get; set; }
        public string FavoritesId { get; set; }
        public string RecipeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public string ImgUrl { get; set; }

        public int PreparationTime { get; set; }

        public double Calories { get; set; }
        public bool IsVegan { get; set; }

        public bool IsVegetarian { get; set; }
        public string CategoryId { get; set; }
        public IndexCategoryVM Category { get; set; }
        public ICollection<IndexIngredientVM> RecipeIngredients { get; set; }
    }
}
