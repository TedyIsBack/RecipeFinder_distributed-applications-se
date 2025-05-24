namespace RecipeFinderMVC.Models
{
    public class PagerModel
    {
        public int TotalCount { get; set; }

        public int PagesCount { get; set; }
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }
    }
}
