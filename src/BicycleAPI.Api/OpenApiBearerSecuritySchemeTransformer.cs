using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace BicycleAPI.Api;

public static class OpenApiBearerSecuritySchemeTransformer
{
    internal sealed class EndpointsHttpSecuritySchemeResolutionTransformer(
        IAuthenticationSchemeProvider authenticationSchemeProvider) : IOpenApiOperationTransformer
    {
        private static string? _defaultSchemeName;

        public async Task TransformAsync(
            OpenApiOperation operation,
            OpenApiOperationTransformerContext context,
            CancellationToken cancellationToken)
        {
            var authorizeAttribute = context.Description.ActionDescriptor.EndpointMetadata
                .OfType<AuthorizeAttribute>()
                .FirstOrDefault();

            if (authorizeAttribute is null)
            {
                return;
            }

            var targetSchemes = authorizeAttribute.AuthenticationSchemes?.Split(',');
            if (targetSchemes is null)
            {
                _defaultSchemeName ??= (await authenticationSchemeProvider.GetDefaultAuthenticateSchemeAsync())?.Name;
                ArgumentException.ThrowIfNullOrWhiteSpace(
                    _defaultSchemeName,
                    "No default authentication scheme found while one was expected."
                );

                operation.Security ??= new List<OpenApiSecurityRequirement>(1);
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(_defaultSchemeName, context.Document)] = []
                });

                return;
            }

            operation.Security ??= new List<OpenApiSecurityRequirement>(targetSchemes.Length);
            foreach (var scheme in targetSchemes.Select(x => x.Trim()))
            {
                ArgumentException.ThrowIfNullOrEmpty(
                    scheme,
                    "Encountered an empty authentication scheme while processing AuthorizeAttribute."
                );
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference(scheme, context.Document)] = []
                });
            }
        }
    }

    public static OpenApiOptions AddEndpointsHttpSecuritySchemeResolution(this OpenApiOptions options)
        => options.AddOperationTransformer<EndpointsHttpSecuritySchemeResolutionTransformer>();

    internal sealed class BearerSecuritySchemeTransformer(
        IAuthenticationSchemeProvider authenticationSchemeProvider)
        : IOpenApiDocumentTransformer
    {
        public async Task TransformAsync(
            OpenApiDocument document, OpenApiDocumentTransformerContext context,
            CancellationToken cancellationToken)
        {
            var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
            if (authenticationSchemes.All(authScheme => authScheme.Name != "Bearer"))
                return;

            document.Components ??= new OpenApiComponents();
            document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
            document.Components.SecuritySchemes[JwtBearerDefaults.AuthenticationScheme] = new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Name = JwtBearerDefaults.AuthenticationScheme,
                Scheme = "bearer",
                In = ParameterLocation.Header,
                BearerFormat = "JSON Web Token",
                Description = "Bearer authentication using a JWT.",
            };
        }
    }

    public static OpenApiOptions AddBearerSecurityScheme(this OpenApiOptions options)
        => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
}
