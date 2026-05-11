using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Lms.Api.OpenApi.Transformers
{
    public class BearerSecuritySchemeTransformer : IOpenApiDocumentTransformer, IOpenApiOperationTransformer
    {
        private const string SchemeId = JwtBearerDefaults.AuthenticationScheme;

        public Task TransformAsync(OpenApiOperation operation, OpenApiOperationTransformerContext context, CancellationToken cancellationToken)
        {
            var authMetadata = context.Description.ActionDescriptor.EndpointMetadata.OfType<IAuthorizeData>();

            if (authMetadata.Any())
            {
                operation.Security ??= [];
                var schemeReference = new OpenApiSecuritySchemeReference(SchemeId);
                var requirement = new OpenApiSecurityRequirement
                {
                    [schemeReference] = []
                    };
                operation.Security.Add(requirement);
            }

            return Task.CompletedTask;
        }

        public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
        {
            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes[SchemeId] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter JWT Bearer token",
                Name = "Authorization"
            };
            return Task.CompletedTask;
        }
    }
}
