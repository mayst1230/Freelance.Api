using Freelance.Core.Models;
using Freelance.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.HttpOverrides;
using Freelance.Api.Services;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Freelance.Api.Services.Fakes;

namespace Freelance.Api.Extensions;

/// <summary>
/// Методы расширения для настройки контейнера DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Настройка служб в контейнере DI.
    /// </summary>
    /// <param name="services">Коллекция сервисов в контейнере DI.</param>
    /// <param name="configuration">Конфигурация среды.</param>
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        ConfigureBase(services, configuration);
        ConfigureDatabase(services, configuration);
        ConfigureApiBase(services);
        ConfigureApiAuthorization(services, configuration);
        ConfigureCustomServices(services, configuration);
        ConfigureSwagger(services);
    }

    /// <summary>
    /// Настройка самописных сервисов.
    /// </summary>
    /// <param name="services">Коллекция сервисов в контейнере DI.</param>
    /// <param name="configuration">Конфигурация среды.</param>
    private static void ConfigureCustomServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<Interfaces.IFileStorage, DataContextFileStorage>();
        services.AddTransient<Interfaces.IUserService, UserService>();
        services.AddTransient<Interfaces.IEmailSender, EmailSenderService>();

        if (configuration.GetValue<bool>("UseFakes") == true)
        {
            services.AddTransient<Interfaces.IPaymentService, FakePaymentService>();
        }
        else
        {
            // ... Add not fake service for "IsNotDevelopment" environment... \\
        }
    }

    /// <summary>
    /// Настройка Swagger.
    /// </summary>
    /// <param name="services">Коллекция сервисов в контейнере DI.</param>
    private static void ConfigureSwagger(IServiceCollection services)
    {
        services.AddSingleton<IConfigureOptions<SwaggerGenOptions>, SwaggerGenApiOptions>();
        services.AddSwaggerGen();
    }

    /// <summary>
    /// Настройка доступа к базе данных.
    /// </summary>
    /// <param name="services">Коллекция сервисов в контейнере DI.</param>
    /// <param name="configuration">Конфигурация среды.</param>
    private static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<DataContext>(options => options.UseNpgsql(configuration.GetConnectionString("DB"), x => x.UseNetTopologySuite()));
    }

    /// <summary>
    /// Настройка базовых сервисов.
    /// </summary>
    /// <param name="services">Коллекция сервисов в контейнере DI.</param>
    /// <param name="configuration">Конфигурация среды.</param>
    private static void ConfigureBase(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders =
                ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });

        services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
    }

    /// <summary>
    /// Настройка базовых сервисов для работы API.
    /// </summary>
    /// <param name="services">Коллекция сервисов в контейнере DI.</param>
    private static void ConfigureApiBase(IServiceCollection services)
    {
        services.AddControllers(options =>
        {
            options.Filters.Add(new ProducesResponseTypeAttribute(StatusCodes.Status200OK));
            options.Filters.Add(new ProducesResponseTypeAttribute(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest));
            options.Filters.Add(new ApiErrorResponseFilter());
            options.OutputFormatters.RemoveType<StringOutputFormatter>();
        })
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });

        services.AddApiVersioning(config =>
        {
            config.ReportApiVersions = true;
            config.ApiVersionReader = new UrlSegmentApiVersionReader();
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.SuppressModelStateInvalidFilter = true;
        });

        services.AddEndpointsApiExplorer();
    }

    /// <summary>
    /// Настройка авторизации для API.
    /// </summary>
    /// <param name="services">Коллекция сервисов в контейнере DI.</param>
    /// <param name="configuration">Конфигурация среды.</param>
    private static void ConfigureApiAuthorization(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = configuration["Jwt:Issuer"],
                        ValidAudience = configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]))
                    };
                });

        services.AddTransient<Interfaces.IJwtHandler, JwtHandlerService>();
    }
}
