namespace RecipeFinderMVC.VIewModels.Ingredients
{
    public class IndexRecipeIngredientVM
    {
        //public string Id { get; set; }         // Идентификатор на рецептат
        public string RecipeIngredientId { get; set; }
        public string IngredientId { get; set; }
        public double Quantity { get; set; }   // Количеството на съставката в рецептата
        public string Name { get; set; }       // Името на съставката
        public string ImgUrl { get; set; }     // Снимката на съставката
        public string Unit { get; set; }       // Единица на измерване
        public double CaloriesPer100g { get; set; }  // Калории на 100г
        public bool? IsAllergen { get; set; }  // Ако е алерген
    }
}
