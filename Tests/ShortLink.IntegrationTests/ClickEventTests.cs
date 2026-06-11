using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ShortLink.Api.DTOs.ClickEvent;
using ShortLink.Api.DTOs.Account;
using ShortLink.Api.DTOs.ShortUrl;
using ShortLink.Application.Features.Account;
using ShortLink.Application.Features.ShortUrl.Commands.CreateShortUrl;
using ShortLink.Application.Features.ClickEvent.Queries.GetByUrlId;
using ShortLink.Application.Features.ClickEvent.Queries.GetDailyClicks;
using ShortLink.Application.Features.ClickEvent.Queries.GetTopReferrers;
using ShortLink.Application.Features.ClickEvent.Queries.GetCountryStats;
using ShortLink.Application.Features.ClickEvent.Queries.GetDeviceStats;
using FluentAssertions;

namespace ShortLink.IntegrationTests;

public class ClickEventTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ClickEventTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task RecordClick_ValidShortCode_Returns200()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);
        var created = await CreateShortUrlAsync("https://example.com");

        var dto = new RecordDto(created.ShortCode, "https://google.com", "127.0.0.1", "Mozilla/5.0");
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/clickevent")
        {
            Content = JsonContent.Create(dto)
        };
        request.Headers.Add("INTERNAL_SECURE_TOKEN", "test-internal-token");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        result.GetProperty("success").GetBoolean().Should().BeTrue();
    }

    [Fact]
    public async Task RecordClick_InvalidShortCode_Returns404()
    {
        var dto = new RecordDto("NONEXISTENT", "", "127.0.0.1", "");
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/clickevent")
        {
            Content = JsonContent.Create(dto)
        };
        request.Headers.Add("INTERNAL_SECURE_TOKEN", "test-internal-token");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RecordClick_MissingInternalToken_Returns401()
    {
        var dto = new RecordDto("test", "", "127.0.0.1", "");
        var response = await _client.PostAsJsonAsync("/api/v1/clickevent", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task RecordClick_InvalidInternalToken_Returns403()
    {
        var dto = new RecordDto("test", "", "127.0.0.1", "");
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/clickevent")
        {
            Content = JsonContent.Create(dto)
        };
        request.Headers.Add("INTERNAL_SECURE_TOKEN", "wrong-token");

        var response = await _client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetByUrlId_WithClicks_Returns200()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);
        var created = await CreateShortUrlAsync("https://example.com");
        await RecordClickAsync(created.ShortCode);

        var response = await _client.GetAsync($"/api/v1/clickevent/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var results = await response.Content.ReadFromJsonAsync<List<GetByUrlIdResponse>>();
        results.Should().NotBeNull();
        results!.Should().Contain(r => r.ShortUrlId == created.Id);
    }

    [Fact]
    public async Task GetByUrlId_NoClicks_Returns200()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);
        var created = await CreateShortUrlAsync("https://example.com");

        var response = await _client.GetAsync($"/api/v1/clickevent/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var results = await response.Content.ReadFromJsonAsync<List<GetByUrlIdResponse>>();
        results.Should().NotBeNull();
        results!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetByUrlId_NonExistentUrl_Returns404()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var response = await _client.GetAsync($"/api/v1/clickevent/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByUrlId_OtherUserUrl_Returns404()
    {
        var tokenA = await GetTokenAsync();
        SetAuthHeader(tokenA);
        var created = await CreateShortUrlAsync("https://example.com");

        var tokenB = await GetTokenAsync();
        SetAuthHeader(tokenB);

        var response = await _client.GetAsync($"/api/v1/clickevent/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByUrlId_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync($"/api/v1/clickevent/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDailyClicks_WithDate_Returns200()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);
        var created = await CreateShortUrlAsync("https://example.com");
        await RecordClickAsync(created.ShortCode);

        var today = DateTime.Today.ToString("yyyy-MM-dd");
        var response = await _client.GetAsync($"/api/v1/clickevent/{created.Id}/daily?date={today}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var results = await response.Content.ReadFromJsonAsync<List<GetDailyClicksResponse>>();
        results.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDailyClicks_NoClicks_Returns200()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);
        var created = await CreateShortUrlAsync("https://example.com");

        var response = await _client.GetAsync($"/api/v1/clickevent/{created.Id}/daily");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var results = await response.Content.ReadFromJsonAsync<List<GetDailyClicksResponse>>();
        results.Should().NotBeNull();
        results!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDailyClicks_NonExistentUrl_Returns404()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var response = await _client.GetAsync($"/api/v1/clickevent/{Guid.NewGuid()}/daily");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetDailyClicks_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync($"/api/v1/clickevent/{Guid.NewGuid()}/daily");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTopReferrers_WithClicks_Returns200()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);
        var created = await CreateShortUrlAsync("https://example.com");
        await RecordClickAsync(created.ShortCode, "https://facebook.com");

        var response = await _client.GetAsync($"/api/v1/clickevent/{created.Id}/top-referrers?limit=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var results = await response.Content.ReadFromJsonAsync<List<GetTopReferrersResponse>>();
        results.Should().NotBeNull();
    }

    [Fact]
    public async Task GetTopReferrers_NoClicks_Returns200()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);
        var created = await CreateShortUrlAsync("https://example.com");

        var response = await _client.GetAsync($"/api/v1/clickevent/{created.Id}/top-referrers?limit=5");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var results = await response.Content.ReadFromJsonAsync<List<GetTopReferrersResponse>>();
        results.Should().NotBeNull();
        results!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTopReferrers_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync($"/api/v1/clickevent/{Guid.NewGuid()}/top-referrers?limit=5");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTopReferrers_NonExistentUrl_Returns404()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var response = await _client.GetAsync($"/api/v1/clickevent/{Guid.NewGuid()}/top-referrers?limit=5");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCountryStats_WithClicks_Returns200()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);
        var created = await CreateShortUrlAsync("https://example.com");
        await RecordClickAsync(created.ShortCode);

        var response = await _client.GetAsync($"/api/v1/clickevent/{created.Id}/country-stats");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var results = await response.Content.ReadFromJsonAsync<List<GetCountryStatsResponse>>();
        results.Should().NotBeNull();
    }

    [Fact]
    public async Task GetCountryStats_NoClicks_Returns200()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);
        var created = await CreateShortUrlAsync("https://example.com");

        var response = await _client.GetAsync($"/api/v1/clickevent/{created.Id}/country-stats");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var results = await response.Content.ReadFromJsonAsync<List<GetCountryStatsResponse>>();
        results.Should().NotBeNull();
        results!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCountryStats_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync($"/api/v1/clickevent/{Guid.NewGuid()}/country-stats");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetDeviceStats_WithMobileUA_Returns200()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);
        var created = await CreateShortUrlAsync("https://example.com");
        await RecordClickAsync(created.ShortCode, "", "Mozilla/5.0 (iPhone; CPU iPhone OS 14_0)");

        var response = await _client.GetAsync($"/api/v1/clickevent/{created.Id}/device-stats");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var results = await response.Content.ReadFromJsonAsync<List<GetDeviceStatsResponse>>();
        results.Should().NotBeNull();
    }

    [Fact]
    public async Task GetDeviceStats_NoClicks_Returns200()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);
        var created = await CreateShortUrlAsync("https://example.com");

        var response = await _client.GetAsync($"/api/v1/clickevent/{created.Id}/device-stats");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var results = await response.Content.ReadFromJsonAsync<List<GetDeviceStatsResponse>>();
        results.Should().NotBeNull();
        results!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetDeviceStats_WithoutAuth_Returns401()
    {
        var response = await _client.GetAsync($"/api/v1/clickevent/{Guid.NewGuid()}/device-stats");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<string> GetTokenAsync()
    {
        var userName = $"user{Guid.NewGuid():N}";
        var email = $"{userName}@test.com";
        var registerRequest = new RegisterRequestDto(userName, email, "SecurePass123!");
        var response = await _client.PostAsJsonAsync("/api/v1/account/register", registerRequest);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        return authResponse!.Token;
    }

    private void SetAuthHeader(string token)
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);
    }

    private async Task<CreateShortUrlResponse> CreateShortUrlAsync(string url)
    {
        var dto = new CreateUrlDto(url);
        var response = await _client.PostAsJsonAsync("/api/v1/shorturl", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<CreateShortUrlResponse>())!;
    }

    private async Task RecordClickAsync(string shortCode, string referrer = "", string userAgent = "")
    {
        var dto = new RecordDto(shortCode, referrer, "127.0.0.1", userAgent);
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/v1/clickevent")
        {
            Content = JsonContent.Create(dto)
        };
        request.Headers.Add("INTERNAL_SECURE_TOKEN", "test-internal-token");
        var response = await _client.SendAsync(request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
