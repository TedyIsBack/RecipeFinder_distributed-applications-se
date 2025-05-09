namespace RecipeFinderAPI.Entities
{
    public class RecipeCategory
    {
        public RecipeCategory()
        {
            RecipeCategoryId = Guid.NewGuid().ToString();
        }
        public string RecipeCategoryId { get; set; }
        public string RecipeId {  get; set; }
        public string CategoryId { get; set; }

        public virtual Recipe Recipe {  get; set; }
        public virtual Category Category {  get; set; }
    }
}
