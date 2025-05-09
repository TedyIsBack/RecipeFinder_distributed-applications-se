namespace RecipeFinderAPI.Infrastructure
{
    public class Pager
    {
        public int TotalCount { get; set; } 
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
