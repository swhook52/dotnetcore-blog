using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace core_blog.api.Core
{
        /// <summary>
        /// Configures the Swagger generation options.
        /// </summary>
        /// <remarks>This allows API versioning to define a Swagger document per API version after the
        /// <see cref="IApiVersionDescriptionProvider"/> service has been resolved from the service container.</remarks>
        public class ConfigureBlogSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
        {
            readonly IApiVersionDescriptionProvider provider;

            /// <summary>
            /// Initializes a new instance of the <see cref="ConfigureSwaggerOptions"/> class.
            /// </summary>
            /// <param name="provider">The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.</param>
            public ConfigureBlogSwaggerOptions(IApiVersionDescriptionProvider provider) => this.provider = provider;

            /// <inheritdoc />
            public void Configure(SwaggerGenOptions options)
            {
                // add a swagger document for each discovered API version
                // note: you might choose to skip or document deprecated API versions differently
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
                }
            }

            static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
            {
                var info = new OpenApiInfo()
                {
                    Title = "dotnet Core Blog API Example",
                    Version = description.ApiVersion.ToString(),
                    Description = "Endpoints for the Blog API",
                    Contact = new OpenApiContact() { Name = "Steven Hook", Email = "steven@hookscode.com" },
                };

                if (description.IsDeprecated)
                {
                    info.Description += ". This API version has been deprecated.";
                }

                return info;
            }
        }

}
