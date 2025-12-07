using AccountAPI.Data;
using AccountAPI.Models;
using AccountAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AccountAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // -----------------------------
            // Configure PostgreSQL Database
            // -----------------------------
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // If running on Render, DATABASE_URL will be set
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            if (!string.IsNullOrEmpty(databaseUrl))
            {
                var uri = new Uri(databaseUrl);

                var userInfo = uri.UserInfo.Split(':');
                var host = uri.Host;
                var port = uri.Port > 0 ? uri.Port : 5432;
                var database = uri.AbsolutePath.TrimStart('/');
                var username = userInfo[0];
                var password = userInfo[1];

                // Optional: parse query parameters for SSL
                var sslMode = "Require"; // default
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                if (!string.IsNullOrEmpty(query.Get("sslmode")))
                {
                    sslMode = query.Get("sslmode");
                }

                connectionString = $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode={sslMode};Trust Server Certificate=true";
            }

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            // -----------------------------
            // Configure services
            // -----------------------------
            builder.Services.AddScoped<AuthService>();
            builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // -----------------------------
            // Run pending migrations automatically
            // -----------------------------
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }

            // -----------------------------
            // Enable Swagger
            // -----------------------------
            app.UseSwagger();
            app.UseSwaggerUI();

            // -----------------------------
            // Redirect root URL "/" to Swagger
            // -----------------------------
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/")
                {
                    context.Response.Redirect("/swagger");
                    return;
                }
                await next();
            });

            // -----------------------------
            // HTTPS redirection only for dev
            // -----------------------------
            if (app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
