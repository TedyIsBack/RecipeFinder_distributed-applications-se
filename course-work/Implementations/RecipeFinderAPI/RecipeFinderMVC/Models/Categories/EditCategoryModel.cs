using System.ComponentModel.DataAnnotations;

namespace RecipeFinderMVC.Models.Categories
{
    public class EditCategoryModel
    {

        public string Id { get; set; } 
        //public string Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Ingredient name must be at least 3 characters long")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required]
        [MaxLength(1000)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Category code must be at least 1 character long")]
        [Display(Name = "Short Code")]
        public string ShortCode { get; set; }


        [Required]
        [Display(Name = "Seasonal")]
        public bool IsSeasonal { get; set; }
    }
}
