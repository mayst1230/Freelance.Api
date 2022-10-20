using Freelance.Api.v1.Users;
using Freelance.Core.Models.Storage;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Freelance.Tests.IntegrationTests;

[TestCaseOrderer("FreelanceTests.Helpers.AlphabeticalTestCaseOrderer", "FreelanceTests.Tests")]
[Collection("Non-Parallel Collection")]
public class ApiUsersTests : IClassFixture<CustomWebApplicationFactory<Program, ApiUsersTests>>
{
    private readonly CustomWebApplicationFactory<Program, ApiUsersTests> _factory;
    private static Guid _userUuid;

    private static JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = {
            new JsonStringEnumConverter()
        },
    };

    public ApiUsersTests(CustomWebApplicationFactory<Program, ApiUsersTests> factory)
    {
        _factory = factory;
    }

    private HttpClient GetHttpClientWithoutToken()
    {
        return _factory.CreateClient();
    }

    private async Task<HttpClient> GetHttpClientWithTokenAsync()
    {
        var httpClient = _factory.CreateClient();
        var fakeAccessTokenResponse = await httpClient.GetAsync("https://localhost:7008/v1/Admins/tokens/fake/");
        var fakeAccessToken = await fakeAccessTokenResponse.Content.ReadFromJsonAsync<string>(_jsonSerializerOptions);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", fakeAccessToken);
        return httpClient;
    }

    [Fact]
    public async Task Test1_Create_User()
    {
        // Arrange
        var httpClient = await GetHttpClientWithTokenAsync();
        var content = JsonContent.Create(new UserRegisterRequest()
        {
            Email = "testAdmin@example.ru",
            FirstName = "Test1",
            MiddleName = "Test2",
            LastName = "Test3",
            Password = "testAdmin",
            UserName = "testAdmin",
            Role = UserRole.Admin,
            PhotoFileId = null,
        }, options: _jsonSerializerOptions);

        // Act
        var responseCreate = await httpClient.PostAsync("https://localhost:7008/v1/Users/register", content);
        responseCreate.EnsureSuccessStatusCode();
        var userCreateInfo = await responseCreate.Content.ReadFromJsonAsync<UserItem>(_jsonSerializerOptions);
        var userDetails = await httpClient.GetFromJsonAsync<UserItem>("https://localhost:7008/v1/Users/" + userCreateInfo?.UniqueIdentifier, _jsonSerializerOptions);

        // Assert
        Assert.NotNull(userCreateInfo);
        Assert.NotNull(userDetails);
        Assert.Equal(userCreateInfo?.UniqueIdentifier, userDetails?.UniqueIdentifier);
        Assert.Equal(userCreateInfo?.UserName, userDetails?.UserName);

        _userUuid = userCreateInfo!.UniqueIdentifier;
    }

    [Fact]
    public async Task Test2_Unauthorize_List_Users()
    {
        // Arrange
        var httpClient = GetHttpClientWithoutToken();

        // Act
        var response = await httpClient.GetAsync("https://localhost:7008/v1/Users/");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Test3_Authorize_List_Users()
    {
        // Arrange
        var httpClient = await GetHttpClientWithTokenAsync();

        // Act
        var listUsers = await httpClient.GetFromJsonAsync<UserListResponse>("https://localhost:7008/v1/Users/", _jsonSerializerOptions);

        // Assert
        Assert.NotNull(listUsers);
        Assert.Equal("test1", listUsers?.Items[0].UserName);
        Assert.Equal("test2", listUsers?.Items[1].UserName);
        Assert.Equal("test3", listUsers?.Items[2].UserName);
    }
}
