using System.ComponentModel.DataAnnotations;

namespace RecipeFinderAPI.Entities
{
    public class User
    {
        public User()
        {
            UserId = Guid.NewGuid().ToString();
        }

        public string UserId { get; set; }


        [Required]
        [MaxLength(50)]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MaxLength(30)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)] 
        public string Password { get; set; }

        [Required]
        [MaxLength(10)]
        public string Role { get; set; }
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }
        public virtual ICollection<FavoriteRecipe> FavoriteRecipe { get; set; }
    }
}
