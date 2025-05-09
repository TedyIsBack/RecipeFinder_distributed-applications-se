
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RecipeFinderAPI.Common;
using RecipeFinderAPI.Data;
using RecipeFinderAPI.Repositories;
using RecipeFinderAPI.Services;
using RecipeFinderAPI.Services.Interfaces;
using System;
using System.Text;

namespace RecipeFinderAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddDbContext<RecipeFinderDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddScoped(typeof(BaseRepository<>));


            builder.Services.AddTransient<IUserService, UserService>();


            builder.Services.AddControllers();
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters()
                   {
                       ValidIssuer = "meal.finder",
                       ValidAudience = "projectmanagement.aspnet.restapi",
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("CHuykfVJNR6be0WxykqLXj+QxSTDlsFvleAXwPNL6pjFBdIM9r04IbXPjOXzcRo1"))
                   };
               });

            /*builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.AdminRole, policy =>
                    policy.RequireRole(Constants.AdminRole));
            });*/

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();



            var app = builder.Build();

            using (var serviceScope = app.Services.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<RecipeFinderDbContext>();
                dbContext.Database.Migrate(); // Apply pending migrations

                await SeedData.SeedAsync(serviceScope.ServiceProvider); // 🔁 Seeder
            }

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication(); 
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
