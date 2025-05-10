namespace RecipeFinderAPI.Infrastructure.DTOs.FavoriteRecipesDTOs
{
    public class ResponseFavoriteRecipeDto
    {
        public string Id { get; set; }
        public string RecipeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PreparationTime { get; set; }
        public double Calories { get; set; }
        public string? Difficulty { get; set; }
        public bool IsVegan { get; set; }
        public bool IsVegetarian { get; set; }
        public string CategoryId {  get; set; }
        public string CategoryName { get; set; }

    }
}
