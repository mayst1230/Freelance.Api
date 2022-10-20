using Freelance.Core.Models;
using Freelance.Tests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;

namespace Freelance.Tests;

public class CustomWebApplicationFactory<TStartup, TTestClass>
: WebApplicationFactory<TStartup> where TStartup : class where TTestClass : class
{
    private static readonly HashSet<string> _dataInitialized = new();
    private static readonly object _locker = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("Serilog:MinimumLevel:Default", "Warning");
        builder.UseSetting("Serilog:WriteToFile", "true");

        builder.ConfigureServices(services =>
        {
            // Remove the app's DataContext registration.
            var dataContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DataContext>));
            if (dataContextDescriptor != null)
            {
                services.Remove(dataContextDescriptor);
            }

            // Add DataContext using an in-memory database for testing.
            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite($"Filename={typeof(TTestClass).Name}.db");
                options.ConfigureWarnings(x => x.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.AmbientTransactionWarning));
            });

            // Build the service provider.
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context (DataContext).
            lock (_locker)
            {
                var testClassName = typeof(TTestClass).Name;
                if (!_dataInitialized.Contains(testClassName))
                {
                    _dataInitialized.Add(testClassName);
                    using var scope = sp.CreateScope();
                    var scopedServices = scope.ServiceProvider;
                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup, TTestClass>>>();

                    try
                    {
                        // Seed the database with test data.
                        Utilities.InitializeDbForTests(scopedServices).GetAwaiter().GetResult();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Ошибка заполнения БД тестовыми данными: {Message}", ex.Message);
                        throw;
                    }
                }
            }
        });
    }
}
