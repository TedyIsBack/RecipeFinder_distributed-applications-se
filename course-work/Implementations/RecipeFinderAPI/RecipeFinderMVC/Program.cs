using RecipeFinderMVC.Security;
using static System.Net.Mime.MediaTypeNames;

namespace RecipeFinderMVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSession();

            builder.Services.AddAuthentication("CookieLogin")
                .AddCookie("CookieLogin", config =>
                {
                    config.Cookie.Name = "UserLoginCookie";
                    config.LoginPath = "/Auth/Login";
                    config.AccessDeniedPath = "/Auth/AccessDenied";
                    config.ExpireTimeSpan = TimeSpan.FromMinutes(5);
                    config.SlidingExpiration = true;
                });
            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            builder.Services.AddTransient<JwtTokenHandler>();

            builder.Services.AddHttpClient("ApiClient", client =>
            {
                var baseUrl = builder.Configuration["ApiSettings:BaseUrl"];
                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    return new HttpClientHandler
                    {
                        AllowAutoRedirect = false
                    };
                })
                .AddHttpMessageHandler<JwtTokenHandler>();


            builder.Services.AddHttpContextAccessor();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

           // app.UseStatusCodePagesWithRedirects("/StatusCode/{0}");


            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
            Console.WriteLine("API base URL: " + builder.Configuration["ApiSettings:BaseUrl"]);

        }
    }
}
