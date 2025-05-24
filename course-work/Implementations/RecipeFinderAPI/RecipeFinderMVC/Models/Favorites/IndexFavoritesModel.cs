

using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RecipeFinderMVC.Models.Favorites
{
    public class IndexFavoritesModel
    {
        public IEnumerable<IndexFavoriteModel> Items { get; set; }

        public string? Name { get; set; }
        public bool? isVegan { get; set; }
        public bool? isVegetarian { get; set; }
        public PagerModel Pager { get; set; }

    }
}
