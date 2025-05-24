namespace RecipeFinderMVC.Models.Users
{
    public class IndexUsersModel
    {
        public IEnumerable<IndexUserModel> Items { get; set; }

        public string? Username { get; set; }
        public bool IsActive { get; set; } = true;

        public PagerModel Pager { get; set; }
    }
}
