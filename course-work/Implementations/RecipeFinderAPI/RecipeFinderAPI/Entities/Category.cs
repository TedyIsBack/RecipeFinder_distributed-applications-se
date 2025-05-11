using System.ComponentModel.DataAnnotations;

namespace RecipeFinderAPI.Entities
{
    public class Category
    {
        public Category()
        {
            CategoryId = Guid.NewGuid().ToString();
        }
        public string CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [MaxLength(10)]
        public string ShortCode { get; set; }
        public bool IsSeasonal { get; set; } 
        public DateTime CreatedAt { get; set; }

        public virtual ICollection<Recipe> Recipes { get; set; }

    }
}
