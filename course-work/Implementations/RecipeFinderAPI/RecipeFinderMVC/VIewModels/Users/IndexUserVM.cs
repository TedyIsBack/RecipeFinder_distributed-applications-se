using RecipeFinderAPI.Entities;

namespace RecipeFinderMVC.VIewModels.Users
{
    public class IndexUserVM
    {

        public string Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string CreatedAt { get; set; }
        public bool IsActive { get; set; }

    }
}
