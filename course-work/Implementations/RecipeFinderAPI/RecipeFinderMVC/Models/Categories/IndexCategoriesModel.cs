
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RecipeFinderMVC.Models.Categories
{
    public class IndexCategoriesModel
    {
        public IEnumerable<IndexCategoryModel> Items { get; set; }

        public string? Name { get; set; }
        public bool? IsSeasonal { get; set; }

        public PagerModel Pager { get; set; }
    }
}
