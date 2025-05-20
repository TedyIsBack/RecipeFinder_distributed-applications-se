namespace RecipeFinderMVC.VIewModels
{
    public class PagerVM
    {
        public int TotalCount { get; set; }

        public int PagesCount { get; set; }
        public int Page { get; set; }
        public int ItemsPerPage { get; set; }
    }
}
