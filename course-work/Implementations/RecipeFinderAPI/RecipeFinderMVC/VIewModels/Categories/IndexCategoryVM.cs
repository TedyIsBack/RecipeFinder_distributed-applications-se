namespace RecipeFinderAPI.Infrastructure.DTOs.CategoryDTOs
{
    public class IndexCategoryVM
    {
        public string Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortCode { get; set; }
        public bool IsSeasonal { get; set; }

    }
}
