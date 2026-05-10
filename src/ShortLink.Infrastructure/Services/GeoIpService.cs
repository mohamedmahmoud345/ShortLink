
using System.Text.Json;
using ShortLink.Application.DTOs.IpResponse;
using ShortLink.Application.Services;

namespace ShortLink.Infrastructure.Services;

public class GeoIpService : IGeoIpService
{
    private readonly HttpClient _client;
    public GeoIpService(HttpClient client)
    {
        _client = client;
    }
    public async Task<string> GetCountryByIpAsync(string ipAddress)
    {

        if (string.IsNullOrEmpty(ipAddress) || ipAddress == "127.0.0.1" || ipAddress == "::1")
            return "Localhost";

        try
        {
            var response = await _client.GetAsync($"json/{ipAddress}");
            if (!response.IsSuccessStatusCode)
                return "Unknown";

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<IpApiResponse>(json);

            if (result != null && result.status == "success")
            {
                return result.country;
            }

        }
        catch
        {
            // fail silent
        }

        return "Unknown";
    }
}
