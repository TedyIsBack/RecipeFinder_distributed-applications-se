using RecipeFinderAPI.Entities;
using System.ComponentModel.DataAnnotations;

namespace RecipeFinderMVC.Models.Users
{
    public class IndexUserModel
    {

        public string Id { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Username")]
        public string Username { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }

        [Display(Name = "Created At")]
        public string CreatedAt { get; set; }

        [Display(Name = "Active status")]
        public bool IsActive { get; set; }

    }
}
