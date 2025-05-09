namespace RecipeFinderAPI.Entities
{
    public class FavoriteRecipe
    {
        public FavoriteRecipe()
        {
            FavoriteRecipeId = Guid.NewGuid().ToString(); 
        }
        public string FavoriteRecipeId { get; set; }
        public string RecipeId {  get; set; }
        public string UserId {  get; set; }

        public virtual User User {  get; set; }
        public virtual Recipe Recipe {  get; set; }
    }
}
