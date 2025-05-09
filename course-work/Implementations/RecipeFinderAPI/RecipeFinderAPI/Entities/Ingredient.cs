namespace RecipeFinderAPI.Entities
{
    public class Ingredient
    {
        public Ingredient()
        {
            IngredientId = new Guid().ToString();
        }
        public string IngredientId { get; set; }
        public string Name { get; set; }
        public double Calories { get; set; }
        public string? Unit { get; set; }
        public bool IsAllergen { get; set; }
        public bool IsOrganic { get; set; }
        public virtual ICollection<RecipeIngredient> RecipeIngredients {  get; set; }
    }
}
