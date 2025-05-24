using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RecipeFinderMVC.Models.Accounts
{
    public class EditAccountModel
    {
        public string Id { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Username must be at least 3 characters long.")]
        [DisplayName("Username")]
        public string Username { get; set; }
    }
}
