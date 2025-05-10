namespace RecipeFinderAPI.Entities
{
    public class Category
    {
        public Category()
        {
            CategoryId = Guid.NewGuid().ToString();
        }
        public string CategoryId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string ShortCode { get; set; }
        public bool IsFeatured { get; set; } //about popularity
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Recipe> Recipes { get; set; }

    }
}
