using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Business.Services;
using Swashbuckle.AspNetCore.Swagger;
using core_blog.api.Core;
using Microsoft.AspNetCore.Mvc;

namespace core_blog.api
{
    public class Startup
    {
        private Assembly[] _assemblies;

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

            _assemblies = new []
            {
                typeof(Startup).GetTypeInfo().Assembly,
                typeof(Business.Services.PostService).GetTypeInfo().Assembly,
                typeof(Domain.Startup).GetTypeInfo().Assembly,
                typeof(Dto.Post).GetTypeInfo().Assembly
            };

            AddAutoMapperClasses(services);
            services.AddAutoMapper(_assemblies);

            Domain.Startup.ConfigureServices(services, Configuration);
            services.AddScoped<IPostService, PostService>();
            services.AddScoped<ICommentService, CommentService>();
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

        private static void AddAutoMapperClasses(IServiceCollection services)
        {
            var allTypes = typeof(Startup).GetTypeInfo().Assembly.ExportedTypes.ToArray();

            var profiles = allTypes
                .Where(t => typeof(Profile).GetTypeInfo().IsAssignableFrom(t.GetTypeInfo()))
                .Where(t => !t.GetTypeInfo().IsAbstract);

            Mapper.Initialize(cfg =>
            {
                foreach (var profile in profiles)
                {
                    cfg.AddProfile(profile);
                }
            });

            var openTypes = new[]
            {
                typeof(IValueResolver<,,>),
                typeof(IMemberValueResolver<,,,>),
                typeof(ITypeConverter<,>)
            };

            foreach (var openType in openTypes)
            {
                foreach (var type in allTypes
                    .Where(t => t.GetTypeInfo().IsClass)
                    .Where(t => !t.GetTypeInfo().IsAbstract)
                    .Where(t => t.ImplementsGenericInterface(openType)))
                {
                    services.AddTransient(type);
                }
            }

            services.AddSingleton(Mapper.Configuration);
            services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<AutoMapper.IConfigurationProvider>(), sp.GetService));
        }
    }
}
