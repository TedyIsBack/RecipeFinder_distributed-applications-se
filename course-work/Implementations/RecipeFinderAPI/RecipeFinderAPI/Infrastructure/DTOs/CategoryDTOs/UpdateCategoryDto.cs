using System.ComponentModel.DataAnnotations;

namespace RecipeFinderAPI.Infrastructure.DTOs.CategoryDTOs
{
    public class UpdateCategoryDto
    {

        //public string Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [MaxLength(10)]
        public string ShortCode { get; set; }

        [Required]
        public bool IsSeasonal { get; set; }
        public string CreatedAt { get; set; }
    }
}
