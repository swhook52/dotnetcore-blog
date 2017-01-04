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
                .AddEntityFramework()
                .AddDbContext<BloggingContext>(p => p.UseSqlServer(configuration["ConnectionStrings:BlogConnectionString"]));
        }
    }
}
