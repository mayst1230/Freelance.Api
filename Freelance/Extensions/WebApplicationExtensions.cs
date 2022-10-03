using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;

namespace Freelance.Api.Extensions;

/// <summary>
/// Методы расширения для веб-приложения.
/// </summary>
public static class WebApplicationExtensions
{
    /// <summary>
    /// Конфигурация Pipeline.
    /// </summary>
    /// <param name="app">Настраиваемое веб-прилоежние.</param>
    /// <param name="configuration">Конфигурация среды.</param>
    public static void Configure(this WebApplication app, IConfiguration configuration)
    {
        if (!app.Environment.IsProduction())
        {
            var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "/swagger/{documentName}/FreelanceApi.json";

                c.PreSerializeFilters.Add((swaggerDoc, httpRequest) =>
                {
                    if (!httpRequest.Headers.ContainsKey("X-Forwarded-Host")) return;

                    var serverUrl = $"{httpRequest.Headers["X-Forwarded-Proto"]}://" +
                        $"{httpRequest.Headers["X-Forwarded-Host"]}" +
                        $"{httpRequest.Headers["X-Forwarded-Prefix"]}";

                    swaggerDoc.Servers = new List<OpenApiServer>()
                    {
                        new OpenApiServer { Url = serverUrl }
                    };
                });
            });

            app.UseSwaggerUI(c =>
            {
                foreach (var description in provider.ApiVersionDescriptions.OrderByDescending(i => i.ApiVersion))
                {
                    c.SwaggerEndpoint($"{description.GroupName}/FreelanceApi.json", description.GroupName.ToUpperInvariant());
                }

                c.ConfigObject.DisplayOperationId = true;
                c.ConfigObject.DefaultModelsExpandDepth = 0;
                c.ConfigObject.Filter = "";
                c.ConfigObject.ShowCommonExtensions = true;
            });
        }

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}
