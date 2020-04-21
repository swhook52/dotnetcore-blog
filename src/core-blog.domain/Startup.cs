using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Domain
{
    public static class Startup
    {
        public static void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services
                .AddEntityFrameworkSqlServer()
                .AddDbContext<BloggingContext>(p => p.UseSqlServer(configuration["ConnectionStrings:BlogConnectionString"]));
        }

        public static void Configure(IApplicationBuilder app)
        {
            // Migrate the database to the latest version automatically on application startup
            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                serviceScope.ServiceProvider.GetService<BloggingContext>().Database.Migrate();
            }
        }
    }
}
