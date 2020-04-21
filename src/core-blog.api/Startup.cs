using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using core_blog.api.Core;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace core_blog.api
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }
        readonly string OnlyConfiguredOrigins = "_OnlyConfiguredOrigins";

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("appsettings.secrets.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddControllers();

            var origins = Configuration.GetValue<string>("CorsOrigins").Split(";");
            services.AddCors(options =>
            {
                options.AddPolicy(OnlyConfiguredOrigins,
                builder =>
                {
                    builder
                        .WithOrigins(origins)
                        .WithMethods("GET", "POST", "PUT", "OPTIONS")
                        .WithHeaders("Origin", "Authorization", "Content-Type");
                });
            });

            services.AddApiVersioning(p =>
            {
                p.ReportApiVersions = true;
                p.AssumeDefaultVersionWhenUnspecified = true;
                p.DefaultApiVersion = new ApiVersion(1, 0);
            });

            services.AddVersionedApiExplorer(
                options =>
                {
                    // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
                    // note: the specified format code will format the version as "'v'major[.minor][-status]"
                    options.GroupNameFormat = "'v'VVV";
                    options.SubstituteApiVersionInUrl = true;
                });

            // Register the Swagger generator, defining 1 or more Swagger documents
            services.AddSwaggerGen(options =>
            {
                // add a custom operation filter which sets default values
                options.OperationFilter<SwaggerConfiguration>();
                options.DocumentFilter<SwaggerConfiguration>();

                // Set the comments path for the Swagger JSON and UI.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var fileName = typeof(Startup).GetTypeInfo().Assembly.GetName().Name + ".xml";
                options.IncludeXmlComments(Path.Combine(basePath, fileName));
            });

            var mappableAssemblies = new[]
            {
                typeof(Startup).GetTypeInfo().Assembly,
                typeof(Business.Services.PostService).GetTypeInfo().Assembly,
                typeof(Domain.Startup).GetTypeInfo().Assembly,
                typeof(Dto.Post).GetTypeInfo().Assembly
            };
            services.AddAutoMapper(mappableAssemblies);

            ConfigureDependencyInjection(services);
        }

        private void ConfigureDependencyInjection(IServiceCollection services)
        {
            services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureBlogSwaggerOptions>();

            Domain.Startup.ConfigureServices(services, Configuration);
            Business.Startup.ConfigureServices(services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();
            app.UseCors(OnlyConfiguredOrigins);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseHttpsRedirection();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(options =>
            {
                // build a swagger endpoint for each discovered API version
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", description.GroupName.ToUpperInvariant());
                }

                options.RoutePrefix = string.Empty;
            });
            Domain.Startup.Configure(app);
        }
    }
}
