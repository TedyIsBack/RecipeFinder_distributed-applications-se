
namespace RecipeFinderMVC.VIewModels.Recipes
{
    public class IndexRecipesVM
    {
        public IEnumerable<IndexRecipeVM> Items { get; set; }

        public string? Name { get; set; }
        public bool? isVegan { get; set; }
        public bool? isVegetarian { get; set; }


        public PagerVM Pager { get; set; }
    }
}
