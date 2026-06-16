using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ShortLink.Api.DTOs.Account;
using ShortLink.Application.Features.Account;
using FluentAssertions;

namespace ShortLink.IntegrationTests;

public class AuthTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    // ---- Register Tests ----

    [Fact]
    public async Task Register_WithValidData_ReturnsAuthResponse()
    {
        var userName = $"user{Guid.NewGuid():N}";
        var email = $"{userName}@test.com";
        var request = new RegisterRequestDto(userName, email, "SecurePass123!");

        var response = await _client.PostAsJsonAsync("/api/v1/account/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        authResponse.Should().NotBeNull();
        authResponse!.UserName.Should().Be(userName);
        authResponse.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns400()
    {
        var userName = $"user{Guid.NewGuid():N}";
        var email = $"{userName}@test.com";
        var request = new RegisterRequestDto(userName, email, "SecurePass123!");

        var firstResponse = await _client.PostAsJsonAsync("/api/v1/account/register", request);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var response = await _client.PostAsJsonAsync("/api/v1/account/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_EmptyUserName_Returns400()
    {
        var email = $"test{Guid.NewGuid():N}@test.com";
        var request = new RegisterRequestDto("", email, "SecurePass123!");

        var response = await _client.PostAsJsonAsync("/api/v1/account/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_EmptyPassword_Returns400()
    {
        var email = $"test{Guid.NewGuid():N}@test.com";
        var request = new RegisterRequestDto("name", email, "");

        var response = await _client.PostAsJsonAsync("/api/v1/account/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_PasswordNoDigit_Returns500()
    {
        var userName = $"user{Guid.NewGuid():N}";
        var email = $"{userName}@test.com";
        var request = new RegisterRequestDto(userName, email, "abcdef");

        var response = await _client.PostAsJsonAsync("/api/v1/account/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task Register_PasswordTooShort_Returns500()
    {
        var userName = $"user{Guid.NewGuid():N}";
        var email = $"{userName}@test.com";
        var request = new RegisterRequestDto(userName, email, "Ab1");

        var response = await _client.PostAsJsonAsync("/api/v1/account/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }

    // ---- Login Tests ----

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsAuthResponse()
    {
        var userName = $"user{Guid.NewGuid():N}";
        var email = $"{userName}@test.com";
        var registerRequest = new RegisterRequestDto(userName, email, "SecurePass123!");
        await _client.PostAsJsonAsync("/api/v1/account/register", registerRequest);

        var loginRequest = new LoginRequestDto(email, "SecurePass123!");
        var response = await _client.PostAsJsonAsync("/api/v1/account/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        authResponse.Should().NotBeNull();
        authResponse!.UserName.Should().Be(userName);
        authResponse.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        var userName = $"user{Guid.NewGuid():N}";
        var email = $"{userName}@test.com";
        var registerRequest = new RegisterRequestDto(userName, email, "SecurePass123!");
        await _client.PostAsJsonAsync("/api/v1/account/register", registerRequest);

        var loginRequest = new LoginRequestDto(email, "WrongPassword1!");
        var response = await _client.PostAsJsonAsync("/api/v1/account/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_NonExistentEmail_Returns401()
    {
        var loginRequest = new LoginRequestDto($"nonexistent{Guid.NewGuid():N}@test.com", "SecurePass123!");
        var response = await _client.PostAsJsonAsync("/api/v1/account/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_EmptyEmail_Returns400()
    {
        var loginRequest = new LoginRequestDto("", "SecurePass123!");
        var response = await _client.PostAsJsonAsync("/api/v1/account/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_EmptyPassword_Returns400()
    {
        var loginRequest = new LoginRequestDto($"test{Guid.NewGuid():N}@test.com", "");
        var response = await _client.PostAsJsonAsync("/api/v1/account/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Login_InvalidEmailFormat_Returns400()
    {
        var loginRequest = new LoginRequestDto("bad", "SecurePass123!");
        var response = await _client.PostAsJsonAsync("/api/v1/account/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    // ---- Token Validation ----

    [Fact]
    public async Task Register_ReturnsToken_WithCorrectClaims()
    {
        var userName = $"user{Guid.NewGuid():N}";
        var email = $"{userName}@test.com";
        var request = new RegisterRequestDto(userName, email, "SecurePass123!");

        var response = await _client.PostAsJsonAsync("/api/v1/account/register", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        authResponse.Should().NotBeNull();
        authResponse!.Token.Split('.').Should().HaveCount(3);

        var payload = DecodeJwtPayload(authResponse.Token);
        HasClaimWithValue(payload, userName).Should().BeTrue();
        HasClaimWithValue(payload, email).Should().BeTrue();
        HasClaimWithValue(payload, "User").Should().BeTrue();
    }

    // ---- Helpers ----

    private static JsonElement DecodeJwtPayload(string token)
    {
        var parts = token.Split('.');
        var payload = parts[1];
        var len = payload.Length;
        var padded = len % 4 == 2 ? payload + "==" :
                     len % 4 == 3 ? payload + "=" :
                     payload;
        padded = padded.Replace('-', '+').Replace('_', '/');
        var bytes = Convert.FromBase64String(padded);
        return JsonSerializer.Deserialize<JsonElement>(bytes);
    }

    private static bool HasClaimWithValue(JsonElement payload, string value)
    {
        foreach (var prop in payload.EnumerateObject())
        {
            if (prop.Value.ValueKind == JsonValueKind.String && prop.Value.GetString() == value)
                return true;
            if (prop.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in prop.Value.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.String && item.GetString() == value)
                        return true;
                }
            }
        }
        return false;
    }
}
