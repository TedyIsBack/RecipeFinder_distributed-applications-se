namespace RecipeFinderMVC.Models
{
    public class PagedResultModel<T>
    {
        public IEnumerable<T> Items { get; set; }

        public int TotalCount { get; set; }
        public int PagesCount { get; set; }
        public int Page { get; set; }
        public int itemsPerPage { get; set; }
    }
}
