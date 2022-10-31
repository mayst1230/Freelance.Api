using Freelance.Api.Services;
using Freelance.Core.Models;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Freelance.Tests.UnitTestsl;

public class DataContextFileStorageTests : IDisposable
{
    private readonly DataContext _dataContext;
    private readonly IUrlHelperFactory _urlHelperFactory;
    private readonly IActionContextAccessor _actionContextAccessor;

    public DataContextFileStorageTests()
    {
        DbContextOptionsBuilder<DataContext> dataContextBuilder = new();
        dataContextBuilder.UseSqlite("Data Source=:memory:");
        _dataContext = new(dataContextBuilder.Options);
        _dataContext.Database.OpenConnection();
        _dataContext.Database.EnsureCreated();
        _urlHelperFactory = new UrlHelperFactory();
        _actionContextAccessor = new ActionContextAccessor();
    }

    public void Dispose()
    {
        _dataContext.Database.EnsureDeleted();
    }

    [Fact]
    public async Task UploadAndGetFilesAsync()
    {
        // Arrange
        DataContextFileStorage service = new(_dataContext, _urlHelperFactory, _actionContextAccessor);
        MemoryStream memoryStream = new(new byte[] { 3, 1, 3, 3, 5 }, false);
        MemoryStream memoryStreamAfterUpload = new();

        // Act
        var fileId = await service.UploadAsync(memoryStream, "type/text", "test1.txt", Core.Models.Storage.FileGroupType.ContractorFile, "test1.txt");
        var file = await _dataContext.Files.FirstAsync(i => i.Id == fileId);

        var details = await service.GetDetailsAsync(file.UniqueIdentifier);
        var (data, mime) = await service.GetContentAsync(file.UniqueIdentifier);

        data.CopyTo(memoryStreamAfterUpload);

        // Assert
        Assert.NotNull(details);
        Assert.Equal("test1.txt", details.DisplayName);
        Assert.Equal("test1.txt", details.FileName);
        Assert.Equal("type/text", mime);
        Assert.Equal(memoryStreamAfterUpload.Length, memoryStream.Length);
    }
}
