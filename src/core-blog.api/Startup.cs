using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Mvc;

namespace core_blog.api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.secrets.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();
            services.AddCors();
            services.AddApiVersioning(p =>
            {
                p.AssumeDefaultVersionWhenUnspecified = true;
                p.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddSwaggerGen(p =>
            {
                p.SwaggerDoc("v1", new Info { Title = "Blog API", Version = "v1" });
            });

            var mappableAssemblies = new[]
            {
                typeof(Startup).GetTypeInfo().Assembly,
                typeof(Business.Services.PostService).GetTypeInfo().Assembly,
                typeof(Domain.Startup).GetTypeInfo().Assembly,
                typeof(Dto.Post).GetTypeInfo().Assembly
            };
            services.AddAutoMapper(mappableAssemblies);

            Domain.Startup.ConfigureServices(services, Configuration);
            Business.Startup.ConfigureServices(services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {

            app.UseMvc();
            app.UseCors(builder => builder.WithOrigins("*"));
            app.UseApiVersioning();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Blog API v1");
                c.RoutePrefix = "swagger";
            });

            Domain.Startup.ConfigureServices(app);
        }
    }
}
