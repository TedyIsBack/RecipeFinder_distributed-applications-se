namespace RecipeFinderMVC.VIewModels.Users
{
    public class IndexUsersVM
    {
        public List<IndexUserVM> Items { get; set; }
 

        public string? Username { get; set; }
        public bool IsActive { get; set; } = true;

        public PagerVM Pager { get; set; }
    }
}
