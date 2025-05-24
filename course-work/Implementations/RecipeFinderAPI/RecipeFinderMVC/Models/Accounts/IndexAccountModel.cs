
using System.ComponentModel;

namespace RecipeFinderMVC.Models.Accounts
{ 
    public class IndexAccountModel
    {
        public string Id { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Username")]
        public string Username { get; set; }

        [DisplayName("Created At")]
        public string CreatedAt { get; set; }

    }
}
