
namespace RecipeFinderMVC.Models.Recipes
{
    public class IndexRecipesModel
    {
        public IEnumerable<IndexRecipeModel> Items { get; set; }

        public string? Name { get; set; }
        public bool? isVegan { get; set; }
        public bool? isVegetarian { get; set; }


        public PagerModel Pager { get; set; }
    }
}
