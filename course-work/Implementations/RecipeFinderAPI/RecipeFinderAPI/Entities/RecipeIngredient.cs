namespace RecipeFinderAPI.Entities
{
    public class RecipeIngredient
    {
        public RecipeIngredient()
        {
            RecipeIngredientId = Guid.NewGuid().ToString(); 
        }
        public string RecipeIngredientId { get; set; }

        public string RecipeId {  get; set; }
        public string IngredientId { get; set;}
        public double Quantity {  get; set; }

        public virtual Recipe Recipe { get; set; }
        public virtual Ingredient Ingredient {  get; set; }
    }
}
