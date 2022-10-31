using Freelance.Api.v1.Users;
using Freelance.Core.Models.Storage;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Freelance.Tests.IntegrationTests;

[Collection("Non-Parallel Collection")]
[TestCaseOrderer("Freelance.Tests.Helpers.AlphabeticalTestCaseOrderer", "Freelance.Tests")]
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
    public async Task Test1_RegisterUser()
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
    public async Task Test2_LoginUser()
    {
        // Arrange
        var httpClient = GetHttpClientWithoutToken();

        var content = JsonContent.Create(new UserLoginRequest()
        {
            Password = "testAdmin",
            UserName = "testAdmin",
        }, options: _jsonSerializerOptions);

        // Act
        var responseLogin = await httpClient.PostAsync("https://localhost:7008/v1/Users/login", content);
        responseLogin.EnsureSuccessStatusCode();
        var userLoginInfo = await responseLogin.Content.ReadFromJsonAsync<UserItem>(_jsonSerializerOptions);

        // Assert
        Assert.NotNull(userLoginInfo);
    }

    [Fact]
    public async Task Test3_EditUser()
    {
        // Arrange
        var httpClient = await GetHttpClientWithTokenAsync();
        var content = JsonContent.Create(new UserEditRequest()
        {
            UserUniqueIdentifier = _userUuid,
            Email = "testAdmin@example.ru",
            FirstName = "Test1",
            MiddleName = "Test2",
            LastName = "Test3",
            Password = "testAdmin123",
            UserName = "testAdmin123",
            PhotoFileId = null,
        }, options: _jsonSerializerOptions);

        // Act
        var userDetailsOld = await httpClient.GetFromJsonAsync<UserItem>("https://localhost:7008/v1/Users/" + _userUuid, _jsonSerializerOptions);
        var responseEdit = await httpClient.PutAsync("https://localhost:7008/v1/Users/edit", content);
        responseEdit.EnsureSuccessStatusCode();
        var userEditInfo = await responseEdit.Content.ReadFromJsonAsync<UserItem>(_jsonSerializerOptions);

        // Assert
        Assert.NotNull(userEditInfo);
        Assert.Equal(userEditInfo?.FirstName, userDetailsOld?.FirstName);
        Assert.Equal(userEditInfo?.MiddleName, userDetailsOld?.MiddleName);
        Assert.Equal(userEditInfo?.LastName, userDetailsOld?.LastName);
        Assert.NotEqual(userEditInfo?.UserName, userDetailsOld?.UserName);
    }

    [Fact]
    public async Task Test4_UnauthorizeListUsers()
    {
        // Arrange
        var httpClient = GetHttpClientWithoutToken();

        // Act
        var response = await httpClient.GetAsync("https://localhost:7008/v1/Users/");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Test5_AuthorizeListUsers()
    {
        // Arrange
        var httpClient = await GetHttpClientWithTokenAsync();

        // Act
        var listUsers = await httpClient.GetFromJsonAsync<UserListResponse>("https://localhost:7008/v1/Users/", _jsonSerializerOptions);

        // Assert
        Assert.NotNull(listUsers);
        Assert.Equal(5, listUsers?.TotalCount);
    }
}
