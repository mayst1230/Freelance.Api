using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace Freelance.Api.Services;

/// <summary>
/// Конфигурация Swagger.
/// </summary>
public class SwaggerGenApiOptions : IConfigureOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public SwaggerGenApiOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// Базовая конфигурация.
    /// </summary>
    /// <param name="options">Опции Swagger.</param>
    public void Configure(SwaggerGenOptions options)
    {
        options.CustomOperationIds(apiDesc =>
        {
            var actionDescriptor = apiDesc.ActionDescriptor as ControllerActionDescriptor;
            if (!apiDesc.TryGetMethodInfo(out var methodInfo)) return null;

            return actionDescriptor != null ? $"{actionDescriptor.ControllerName}_{actionDescriptor.ActionName}" : null;
        });

        options.SupportNonNullableReferenceTypes();

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        options.IncludeXmlComments(xmlPath, true);

        foreach (var description in _provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, CreateInfoForApiVersion(description));
        }

        options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme()
        {
            Scheme = JwtBearerDefaults.AuthenticationScheme,
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Name = "Authorization",
            Description = "Bearer Authentication with JWT Token",
            Type = SecuritySchemeType.Http
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                },
                new List<string>()
            }
        });

        options.DocumentFilter<OrderTagsDocumentFilter>();
    }

    /// <summary>
    /// Базовая информация.
    /// </summary>
    /// <param name="description">Описание API.</param>
    /// <returns>Базовую информацию.</returns>
    static OpenApiInfo CreateInfoForApiVersion(ApiVersionDescription description)
    {
        var info = new OpenApiInfo()
        {
            Title = "Freelance API",
            Version = description.ApiVersion.ToString(),
            Contact = new OpenApiContact() 
            { 
                Name = "Yury Mayorov", 
                Email = "mayst1230@gmail.com" 
            },
        };

        return info;
    }
}

/// <summary>
/// Фильтрация методов.
/// </summary>
public class OrderTagsDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags = swaggerDoc.Tags
             .OrderBy(x => x.Name).ToList();
    }
}
