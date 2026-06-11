using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using ShortLink.Api.DTOs.Account;
using ShortLink.Api.DTOs.ShortUrl;
using ShortLink.Application.Features.Account;
using ShortLink.Application.Features.ShortUrl.Commands.CreateShortUrl;
using ShortLink.Application.Features.ShortUrl.Queries.GetById;
using ShortLink.Infrastructure.Data;
using FluentAssertions;

namespace ShortLink.IntegrationTests;

public class ShortUrlTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public ShortUrlTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    // ---- Create Tests ----

    [Fact]
    public async Task Create_WithValidUrl_Returns201()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var dto = new CreateUrlDto("https://example.com");
        var response = await _client.PostAsJsonAsync("/api/v1/shorturl", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CreateShortUrlResponse>();
        result.Should().NotBeNull();
        result!.OriginalLink.Should().Be("https://example.com");
        result.ShortCode.Should().NotBeNullOrEmpty();
        result.IsActive.Should().BeTrue();
        result.Clicks.Should().Be(0);
    }

    [Fact]
    public async Task Create_WithoutAuth_Returns401()
    {
        var dto = new CreateUrlDto("https://example.com");
        var response = await _client.PostAsJsonAsync("/api/v1/shorturl", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Create_WithEmptyUrl_Returns201()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var dto = new CreateUrlDto("");
        var response = await _client.PostAsJsonAsync("/api/v1/shorturl", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    // ---- Get By ID Tests ----

    [Fact]
    public async Task GetById_ExistingId_Returns200()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var created = await CreateShortUrlAsync("https://example.com");
        var response = await _client.GetAsync($"/api/v1/shorturl/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<QueryResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(created.Id);
        result.OriginalLink.Should().Be("https://example.com");
        result.ShortCode.Should().Be(created.ShortCode);
    }

    [Fact]
    public async Task GetById_NonExistentId_Returns404()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var response = await _client.GetAsync($"/api/v1/shorturl/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---- Get By ShortCode Tests ----

    [Fact]
    public async Task GetByShortCode_Existing_Returns200()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var created = await CreateShortUrlAsync("https://example.com");
        var response = await _client.GetAsync($"/api/v1/shorturl/{created.ShortCode}/url");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<QueryResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().Be(created.Id);
        result.ShortCode.Should().Be(created.ShortCode);
    }

    [Fact]
    public async Task GetByShortCode_NonExistent_Returns404()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var response = await _client.GetAsync("/api/v1/shorturl/NONEXISTENT/url");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---- Get Mine Tests ----

    [Fact]
    public async Task GetMine_WithUrls_Returns200()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        await CreateShortUrlAsync("https://example.com");
        await CreateShortUrlAsync("https://test.com");

        var response = await _client.GetAsync("/api/v1/shorturl/mine");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var results = await response.Content.ReadFromJsonAsync<List<QueryResponse>>();
        results.Should().NotBeNull();
        results!.Count.Should().BeGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task GetMine_NoUrls_Returns200()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var response = await _client.GetAsync("/api/v1/shorturl/mine");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var results = await response.Content.ReadFromJsonAsync<List<QueryResponse>>();
        results.Should().NotBeNull();
        results!.Should().BeEmpty();
    }

    // ---- Update Tests ----

    [Fact]
    public async Task Update_OwnUrl_Returns204()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var created = await CreateShortUrlAsync("https://example.com");
        var content = new StringContent("\"https://updated-url.com\"", Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/v1/shorturl/{created.Id}", content);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent); 
    }

    [Fact]
    public async Task Update_NonExistentUrl_Returns404()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var content = new StringContent("\"https://updated-url.com\"", Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/v1/shorturl/{Guid.NewGuid()}", content);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_OtherUserUrl_Returns404()
    {
        var tokenA = await GetTokenAsync();
        SetAuthHeader(tokenA);
        var created = await CreateShortUrlAsync("https://example.com");

        var tokenB = await GetTokenAsync();
        SetAuthHeader(tokenB);
        var content = new StringContent("\"https://updated-url.com\"", Encoding.UTF8, "application/json");
        var response = await _client.PutAsync($"/api/v1/shorturl/{created.Id}", content);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---- Delete Tests ----

    [Fact]
    public async Task Delete_OwnUrl_Returns204()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var created = await CreateShortUrlAsync("https://example.com");
        var response = await _client.DeleteAsync($"/api/v1/shorturl/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_NonExistentUrl_Returns404()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var response = await _client.DeleteAsync($"/api/v1/shorturl/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_OtherUserUrl_Returns404()
    {
        var tokenA = await GetTokenAsync();
        SetAuthHeader(tokenA);
        var created = await CreateShortUrlAsync("https://example.com");

        var tokenB = await GetTokenAsync();
        SetAuthHeader(tokenB);
        var response = await _client.DeleteAsync($"/api/v1/shorturl/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---- Refresh Tests ----

    [Fact]
    public async Task Refresh_ExpiredLink_Returns204()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var created = await CreateShortUrlAsync("https://example.com");

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var url = await db.ShortUrls.FindAsync(created.Id);
            url!.ExpiresAt = DateTime.UtcNow.AddDays(-1);
            url.IsActive = false;
            await db.SaveChangesAsync();            
        }

        var response = await _client.PostAsync($"/api/v1/shorturl/{created.Id}/refresh", null);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Refresh_ActiveLink_Returns400()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var created = await CreateShortUrlAsync("https://example.com");

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var url = await db.ShortUrls.FindAsync(created.Id);
            url!.ExpiresAt = DateTime.UtcNow.AddDays(5);
            url.IsActive = false;
            await db.SaveChangesAsync();
        }

        var response = await _client.PostAsync($"/api/v1/shorturl/{created.Id}/refresh", null);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Refresh_NonExistentLink_Returns404()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var response = await _client.PostAsync($"/api/v1/shorturl/{Guid.NewGuid()}/refresh", null);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // ---- Inactive Links Tests ----

    [Fact]
    public async Task GetInactive_AfterDelete_Returns200()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var created = await CreateShortUrlAsync("https://example.com");

        await _client.DeleteAsync($"/api/v1/shorturl/{created.Id}");

        var response = await _client.GetAsync("/api/v1/shorturl/inactive");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var results = await response.Content.ReadFromJsonAsync<List<QueryResponse>>();
        results.Should().NotBeNull();
        results!.Should().Contain(r => r.Id == created.Id);
    }

    [Fact]
    public async Task GetInactive_NoInactive_Returns200()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var response = await _client.GetAsync("/api/v1/shorturl/inactive");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var results = await response.Content.ReadFromJsonAsync<List<QueryResponse>>();
        results.Should().NotBeNull();
        results!.Should().BeEmpty();
    }

    // ---- Admin Endpoint Tests ----

    [Fact]
    public async Task GetAdminUsers_WithoutAdminRole_Returns403()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var response = await _client.GetAsync("/api/v1/shorturl/admin/users");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAdminUserUrls_WithoutAdminRole_Returns403()
    {
        var token = await GetTokenAsync();
        SetAuthHeader(token);

        var response = await _client.GetAsync($"/api/v1/shorturl/admin/user/{Guid.NewGuid()}/urls");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task GetAdminUsers_WithAdminRole_Returns200()
    {
        var token = await GetAdminTokenAsync();
        SetAuthHeader(token);

        var response = await _client.GetAsync("/api/v1/shorturl/admin/users");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAdminUserUrls_WithAdminRole_Returns200()
    {
        var regularToken = await GetTokenAsync();
        SetAuthHeader(regularToken);
        var created = await CreateShortUrlAsync("https://example.com");
        var regularUserId = GetUserIdFromToken(regularToken);

        var adminToken = await GetAdminTokenAsync();
        SetAuthHeader(adminToken);

        var response = await _client.GetAsync($"/api/v1/shorturl/admin/user/{regularUserId}/urls");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var results = await response.Content.ReadFromJsonAsync<List<QueryResponse>>();
        results.Should().NotBeNull();
        results!.Should().Contain(r => r.Id == created.Id);
    }

    [Fact]
    public async Task GetAdminUserUrls_InvalidUserId_Returns400()
    {
        var token = await GetAdminTokenAsync();
        SetAuthHeader(token);

        var response = await _client.GetAsync("/api/v1/shorturl/admin/user/not-a-guid/urls");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ---- Helpers ----

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

    private async Task<string> GetAdminTokenAsync()
    {
        var loginRequest = new LoginRequestDto("admin@shortlink.com", "SecureAdminPass123!");
        var response = await _client.PostAsJsonAsync("/api/v1/account/login", loginRequest);
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

    private static string GetUserIdFromToken(string token)
    {
        var parts = token.Split('.');
        var payload = parts[1];
        var len = payload.Length;
        var padded = len % 4 == 2 ? payload + "==" :
                     len % 4 == 3 ? payload + "=" :
                     payload;
        padded = padded.Replace('-', '+').Replace('_', '/');
        var bytes = Convert.FromBase64String(padded);
        using var doc = JsonDocument.Parse(bytes);
        foreach (var prop in doc.RootElement.EnumerateObject())
        {
            if (prop.Value.ValueKind == JsonValueKind.String &&
                Guid.TryParse(prop.Value.GetString(), out _))
            {
                return prop.Value.GetString()!;
            }
        }
        throw new InvalidOperationException("User ID claim not found in token");
    }
}
