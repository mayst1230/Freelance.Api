using Freelance.Core.Models;
using Freelance.Core.Models.Storage;
using Microsoft.EntityFrameworkCore;

namespace Freelance.Tests.Helpers;

public static class Utilities
{
    private static int feedbackCreatedById;
    private static int feedbackUserId;
    public static async Task InitializeDbForTests(IServiceProvider services)
    {
        var dataContext = services.GetRequiredService<DataContext>();

        dataContext.Database.EnsureDeleted();
        dataContext.Database.EnsureCreated();

        await AddUsers(dataContext);
        await AddFeedbacks(dataContext);
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
                Rating = 0m,
                Created = DateTimeOffset.UtcNow,
                Updated = DateTimeOffset.UtcNow,
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
                Rating = 0m,
                Created = DateTimeOffset.UtcNow,
                Updated = DateTimeOffset.UtcNow,
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
                Rating = 0m,
                Created = DateTimeOffset.UtcNow,
                Updated = DateTimeOffset.UtcNow,
            },
            new User()
            {
                Id = 10,
                UniqueIdentifier = Guid.Parse("7935ccc5-6b4a-4d5f-a8ef-cb2e4e404598"),
                UserName = "fakeUser",
                FirstName = "fakeUser",
                LastName = "fakeUser",
                MiddleName = "fakeUser",
                Email = "fakeUserEmail",
                Password = "fakeUser",
                Role = UserRole.Admin,
                Rating = 0m,
                Created = DateTimeOffset.UtcNow,
                Updated = DateTimeOffset.UtcNow,
            },
        };

        await dataContext.Users.AddRangeAsync(users);
        await dataContext.SaveChangesAsync();

        feedbackUserId = await dataContext.Users.Where(i => i.UserName == "test2").Select(i => i.Id).FirstOrDefaultAsync();
        feedbackCreatedById = await dataContext.Users.Where(i => i.UserName == "test3").Select(i => i.Id).FirstOrDefaultAsync();
    }

    private static async Task AddFeedbacks(DataContext dataContext)
    {
        var feedbacks = new Feedback[] {
            new Feedback()
            {
                CreatedBy = feedbackCreatedById,
                UserId = feedbackUserId,
                Text = "Отзыв 1",
                UserRating = 4.5m,
                Created = DateTimeOffset.UtcNow,
                Updated = DateTimeOffset.UtcNow,
            },
            new Feedback()
            {
                CreatedBy = feedbackUserId,
                UserId = feedbackCreatedById,
                Text = "Отзыв 2",
                UserRating = 4.0m,
                Created = DateTimeOffset.UtcNow,
                Updated = DateTimeOffset.UtcNow,
            },
        };

        await dataContext.Feedbacks.AddRangeAsync(feedbacks);
        await dataContext.SaveChangesAsync();
    }
}
