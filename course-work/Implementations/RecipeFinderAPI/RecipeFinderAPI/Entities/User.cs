namespace RecipeFinderAPI.Entities
{
    public class User
    {
        public User()
        {
            UserId = Guid.NewGuid().ToString();
        }

        public string UserId { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } //admin or user
        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual ICollection<FavoriteRecipe> FavoriteRecipe { get; set; }
    }
}
