using Freelance.Api.v1.Feedbacks;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit;

namespace Freelance.Tests.IntegrationTests;

[Collection("Non-Parallel Collection")]
[TestCaseOrderer("Freelance.Tests.Helpers.AlphabeticalTestCaseOrderer", "Freelance.Tests")]
public class ApiFeedbacksTests : IClassFixture<CustomWebApplicationFactory<Program, ApiFeedbacksTests>>
{
    private readonly CustomWebApplicationFactory<Program, ApiFeedbacksTests> _factory;
    private static Guid _feedbackUuid;

    private static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = {
            new JsonStringEnumConverter()
        },
    };

    public ApiFeedbacksTests(CustomWebApplicationFactory<Program, ApiFeedbacksTests> factory)
    {
        _factory = factory;
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
    public async Task Test1_CreateFeedback()
    {
        // Arrange
        var httpClient = await GetHttpClientWithTokenAsync();
        var content = JsonContent.Create(new FeedbackCreateRequest()
        {
            UserUniqueIdentifier = Guid.Parse("7935ccc5-6b4a-4d5f-a8ef-cb2e4e404598"),
            Text = "123",
            UserRating = 4.5m,
        }, options: _jsonSerializerOptions);

        // Act
        var responseCreate = await httpClient.PostAsync("https://localhost:7008/v1/Feedbacks/create", content);
        responseCreate.EnsureSuccessStatusCode();
        var feedbackCreateInfo = await responseCreate.Content.ReadFromJsonAsync<FeedbackItem>(_jsonSerializerOptions);
        var feedbackDetails = await httpClient.GetFromJsonAsync<FeedbackItem>("https://localhost:7008/v1/Feedbacks/" + feedbackCreateInfo?.UniqueIdentifier, _jsonSerializerOptions);

        // Assert
        Assert.NotNull(feedbackCreateInfo);
        Assert.Equal(feedbackCreateInfo?.UniqueIdentifier, feedbackDetails?.UniqueIdentifier);
        _feedbackUuid = feedbackCreateInfo!.UniqueIdentifier;
    }

    [Fact]
    public async Task Test2_EditFeedback()
    {
        // Arrange
        var httpClient = await GetHttpClientWithTokenAsync();
        var content = JsonContent.Create(new FeedbackEditRequest()
        {
            FeedbackUuid = _feedbackUuid,
            Text = "Отзыв 11",
            UserRating = 4.5m,
        }, options: _jsonSerializerOptions);

        // Act
        var feedbackDetailsOld = await httpClient.GetFromJsonAsync<FeedbackItem>("https://localhost:7008/v1/Feedbacks/" + _feedbackUuid, _jsonSerializerOptions);
        var responseEdit = await httpClient.PutAsync("https://localhost:7008/v1/Feedbacks/edit", content);
        responseEdit.EnsureSuccessStatusCode();
        var feedbackEditInfo = await responseEdit.Content.ReadFromJsonAsync<FeedbackItem>(_jsonSerializerOptions);

        // Assert
        Assert.NotNull(feedbackEditInfo);
        Assert.NotEqual(feedbackEditInfo?.Text, feedbackDetailsOld?.Text);
        Assert.Equal(feedbackEditInfo?.UserRating, feedbackDetailsOld?.UserRating);
    }
}
