using System.Reflection;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServerHub.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityServerHub
{
    public class Program
    {
        private static void InitializeDatabase(IHost host)
        {
            using var serviceScope = host.Services.CreateScope();
            var services = serviceScope.ServiceProvider;

            var configurationContext = services.GetRequiredService<ConfigurationDbContext>();

            if (!configurationContext.Clients.Any())
            {
                foreach (var client in Config.Clients)
                {
                    configurationContext.Clients.Add(client.ToEntity());
                }
                configurationContext.SaveChanges();
            }

            if (!configurationContext.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources)
                {
                    configurationContext.IdentityResources.Add(resource.ToEntity());
                }
                configurationContext.SaveChanges();
            }

            if (!configurationContext.ApiScopes.Any())
            {
                foreach (var apiScope in Config.ApiScopes)
                {
                    configurationContext.ApiScopes.Add(apiScope.ToEntity());
                }
                configurationContext.SaveChanges();
            }

            if (!configurationContext.ApiResources.Any())
            {
                foreach (var resource in Config.ApiResources)
                {
                    configurationContext.ApiResources.Add(resource.ToEntity());
                }
                configurationContext.SaveChanges();
            }
        }

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;
            string connectionString = configuration.GetConnectionString("IdentityHub");

            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

            builder.Services.AddDbContext<ConfigurationDbContext>(options =>
                options.UseSqlServer(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));

            builder.Services.AddIdentityServer()
                .AddTestUsers(Config.TestUsers)
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddDeveloperSigningCredential();

            builder.Services.AddControllersWithViews();
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Seed the database
            InitializeDatabase(app.Services.GetService<IHost>());

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.MapDefaultControllerRoute(); 
            app.MapRazorPages();

            app.Run();
        }
    }
}
