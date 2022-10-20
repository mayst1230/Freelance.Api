using Freelance.Core.Models;
using Freelance.Core.Models.Storage;

namespace Freelance.Tests.Helpers;

public static class Utilities
{
    public static async Task InitializeDbForTests(IServiceProvider services)
    {
        var dataContext = services.GetRequiredService<DataContext>();

        dataContext.Database.EnsureDeleted();
        dataContext.Database.EnsureCreated();

        await AddUsers(dataContext);
    }
    private static async Task AddUsers(DataContext dataContext)
    {
        var users = new User[] {
            new User()
            {
                UserName = "test1",
                Email = "test1@example.com",
                FirstName = "test1",
                LastName = "test1",
                MiddleName = "test1",
                Role = UserRole.Admin,
                Password = "123",
                Created = DateTimeOffset.Now,
            },
            new User()
            {
                UserName = "test2",
                Email = "test2@example.com",
                FirstName = "test2",
                LastName = "test2",
                MiddleName = "test2",
                Role = UserRole.Contractor,
                Password = "123",
                Created = DateTimeOffset.Now,
            },
            new User()
            {
                UserName = "test3",
                Email = "test3@example.com",
                FirstName = "test3",
                LastName = "test3",
                MiddleName = "test3",
                Role = UserRole.Customer,
                Password = "123",
                Created = DateTimeOffset.Now,
            }
        };

        dataContext.Users.AddRange(users);
        await dataContext.SaveChangesAsync();
    }

}
