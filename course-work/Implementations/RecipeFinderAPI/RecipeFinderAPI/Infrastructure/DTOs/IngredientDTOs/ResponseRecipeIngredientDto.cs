namespace RecipeFinderAPI.Infrastructure.DTOs.IngredientDTOs
{
    public class ResponseRecipeIngredientDto
    {
        //public string RecipeIngredientId { get; set; }
        public string IngredientId { get; set; }
        public double Quantity { get; set; }   
        public string Name { get; set; }       
        public string ImgUrl { get; set; }     
        public string Unit { get; set; }       
        public double CaloriesPer100g { get; set; }  
        public bool? IsAllergen { get; set; }  
    }
}
