namespace RecipeFinderMVC.VIewModels.Ingredients
{
    public class IndexIngredientsVM
    {
        public IEnumerable<IndexIngredientVM> Items { get; set; }

        public string? Name { get; set; }
        public bool? isAllergen { get; set; } 

        public PagerVM Pager { get; set; }
    }

}
