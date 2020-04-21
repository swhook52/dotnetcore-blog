using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Text.RegularExpressions;

namespace core_blog.api.Core
{

    /// <summary>
    /// Represents the Swagger/Swashbuckle operation filter used to document the implicit API version parameter.
    /// </summary>
    /// <remarks>This <see cref="IOperationFilter"/> is only required due to bugs in the <see cref="SwaggerGenerator"/>.
    /// Once they are fixed and published, this class can be removed.</remarks>
    public class SwaggerConfiguration : IOperationFilter, IDocumentFilter
    {
        /// <summary>
        /// Applies the filter to the specified operation using the given context.
        /// </summary>
        /// <param name="operation">The operation to apply the filter to.</param>
        /// <param name="context">The current operation filter context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            operation.Deprecated |= apiDescription.IsDeprecated();

            if (operation.Parameters == null)
            {
                return;
            }

            // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/412
            // REF: https://github.com/domaindrivendev/Swashbuckle.AspNetCore/pull/413
            foreach (var parameter in operation.Parameters)
            {
                var description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);

                if (parameter.Description == null)
                {
                    parameter.Description = description.ModelMetadata?.Description;
                }

                if (parameter.Schema.Default == null && description.DefaultValue != null)
                {
                    parameter.Schema.Default = new OpenApiString(description.DefaultValue.ToString());
                }

                parameter.Required |= description.IsRequired;
            }
        }

        public void Apply(OpenApiDocument document, DocumentFilterContext context)
        {
            // This is optional given your situation. Not removing it means Swagger will work in local developemnt.
            // RemoveOperationPrefix(document);
        }

        private static void RemoveOperationPrefix(OpenApiDocument document)
        {
            // There are instances where you may want to create a different route for each operation.
            // One reason to do this is if you manage this api through Azure API Management.
            // Removing /api/vi from swagger means your APIM endpoint won't look like /Blog/api/v1/api/vi/Posts
            var newPaths = new OpenApiPaths();
            foreach (var path in document.Paths)
            {
                var newPath = Regex.Replace(path.Key, @"/api/v\d+/", "/");
                newPaths.Add(newPath, path.Value);
            }

            document.Paths = newPaths;
        }
    }

}
