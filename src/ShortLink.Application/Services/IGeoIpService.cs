
namespace ShortLink.Application.Services;

public interface IGeoIpService
{
    Task<string> GetCountryByIpAsync(string ipAddress);
}
