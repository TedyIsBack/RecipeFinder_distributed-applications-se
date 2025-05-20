using RecipeFinderMVC.VIewModels.Categories;

namespace RecipeFinderMVC.VIewModels.Categories
{
    public class IndexCategoriesVM
    {
        public IEnumerable<IndexCategoryVM> Items { get; set; }

        public string? Name { get; set; }
        public bool IsSeasonal { get; set; } = true;

        public PagerVM Pager { get; set; }
    }
}
