namespace RecipeFinderMVC.Models.Ingredients
{
    public class IndexIngredientsModel
    {
        public IEnumerable<IndexIngredientModel> Items { get; set; }

        public string? Name { get; set; }
        public bool? isAllergen { get; set; } 

        public PagerModel Pager { get; set; }
    }

}
