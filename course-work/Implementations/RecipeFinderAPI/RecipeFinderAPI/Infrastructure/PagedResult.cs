namespace RecipeFinderAPI.Infrastructure
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }

        public int TotalCount { get; set; }
        public int PagesCount { get; set; }
        public int Page { get; set; }
        public int itemsPerPage { get; set; }
    }
}
