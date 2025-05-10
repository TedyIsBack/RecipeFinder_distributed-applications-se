using RecipeFinderAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace RecipeFinderAPI.Data
{
    public class RecipeFinderDbContext : DbContext
    {
        public RecipeFinderDbContext(DbContextOptions<RecipeFinderDbContext> options)
            : base(options)
        {
        }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<FavoriteRecipe> FavoriteRecipes { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Users

            modelBuilder.Entity<User>()
                .HasKey(x => x.UserId);

            #endregion

            #region Recipes

            modelBuilder.Entity<Recipe>()
                .HasKey(x => x.RecipeId);

            modelBuilder.Entity<Recipe>()
                 .HasOne(x => x.Category)
                 .WithMany(x => x.Recipes)
                 .HasForeignKey(x => x.CategoryId)
                 .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Ingredients

            modelBuilder.Entity<Ingredient>()
                .HasKey(x => x.IngredientId);

            #endregion

            #region Categories

            modelBuilder.Entity<Category>()
                .HasKey(x => x.CategoryId);

            #endregion

            #region FavoriteRecipes
            modelBuilder.Entity<FavoriteRecipe>()
                .HasKey(x => x.FavoriteRecipeId);

            modelBuilder.Entity<FavoriteRecipe>()
                .HasOne(x => x.User)
                .WithMany(x => x.FavoriteRecipe)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FavoriteRecipe>()
                .HasOne(x => x.Recipe)
                .WithMany(x => x.FavoriteRecipes)
                .HasForeignKey(x => x.RecipeId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region RecipeIngredients

            modelBuilder.Entity<RecipeIngredient>()
                .HasKey(x => x.RecipeIngredientId);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(x => x.Recipe)
                .WithMany(x => x.RecipeIngredients)
                .HasForeignKey(x => x.RecipeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RecipeIngredient>()
                .HasOne(x => x.Ingredient)
                .WithMany(x => x.RecipeIngredients)
                .HasForeignKey(x => x.IngredientId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
