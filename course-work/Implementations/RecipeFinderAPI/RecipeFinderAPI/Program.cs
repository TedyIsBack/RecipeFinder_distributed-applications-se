﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RecipeFinderAPI.Common;
using RecipeFinderAPI.Data;
using RecipeFinderAPI.Infrastructure;
using RecipeFinderAPI.Repositories;
using RecipeFinderAPI.Services;
using RecipeFinderAPI.Services.Interfaces;
using System.Reflection;
using System.Text;

namespace RecipeFinderAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add database context
            builder.Services.AddDbContext<RecipeFinderDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register repositories and services
            builder.Services.AddScoped(typeof(BaseRepository<>));
            builder.Services.AddTransient<IUserService, UserService>();
            builder.Services.AddTransient<IAccountService, AccountService>();
            builder.Services.AddTransient<IAuthService, AuthService>();
            builder.Services.AddTransient<IIngredientService, IngredientService>();
            builder.Services.AddTransient<ICategoryService, CategoryService>();
            builder.Services.AddTransient<IFavoriteService, FavoriteService>();
            builder.Services.AddTransient<IRecipeService, RecipeService>();

            builder.Services.AddScoped<TokenService>();
            builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

            // JWT Configuration: Read values from appsettings.json
            var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

            // Register JwtSettings for DI (optional for direct access in your app)
            builder.Services.AddSingleton(jwtSettings);

            // Add Authentication middleware with JWT Bearer
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                    };
                });

            // Add controllers and configure CORS policy
            //builder.Services.AddControllers();
            builder.Services.AddControllers(options =>
            {
                options.ReturnHttpNotAcceptable = true; 
            });

            // CORS configuration
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("https://localhost:7227") 
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.IncludeXmlComments(Path.Combine(
                AppContext.BaseDirectory,
                 $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"), true);

                options.SwaggerDoc("default", new OpenApiInfo
                {
                    Title = "RecipeFinderAPI",
                    Version = "default",
                    Description = @"RecipeFinder lets users search, add, and manage recipes easily. It's built to help you find meals based on basic preferences.

- [GitHub Repo](https://github.com/TedyIsBack/RecipeFinder_distributed-applications-se/tree/master/course-work/Implementations/RecipeFinderAPI)"
                });


                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Name = "Bearer",
                            In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });
            });

            var app = builder.Build();

            // Run migrations and seed data
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<RecipeFinderDbContext>();
                dbContext.Database.Migrate();
                await SeedData.SeedAsync(scope.ServiceProvider);
            }

            // Middleware pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                //app.UseSwaggerUI();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/default/swagger.json", "RecipeFinderAPI");
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // Enable CORS and Authentication
            //app.UseHttpsRedirection();
            app.UseCors("AllowFrontend");  // Apply CORS policy
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
