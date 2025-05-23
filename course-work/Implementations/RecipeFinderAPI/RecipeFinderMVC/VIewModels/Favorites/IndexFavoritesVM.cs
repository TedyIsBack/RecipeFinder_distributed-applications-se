

namespace RecipeFinderMVC.VIewModels.Favorites
{
    public class IndexFavoritesVM
    {
        public IEnumerable<IndexFavoriteVM> Items { get; set; }

        public string? Name { get; set; }
        public bool? isVegan { get; set; }
        public bool? isVegetarian { get; set; }
        public PagerVM Pager { get; set; }

    }
}
