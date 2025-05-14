using System.ComponentModel.DataAnnotations;

namespace RecipeFinderAPI.Infrastructure.DTOs.CategoryDTOs
{
    public class CreateCategoryDto
    {

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Ingredient name must be at least 3 characters long")]
        public string Name { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Category code must be at least 1 character long")]
        public string ShortCode { get; set; }

        [Required]
        public bool IsSeasonal { get; set; }
    }
}
