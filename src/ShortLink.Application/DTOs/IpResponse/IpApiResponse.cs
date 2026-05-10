using System;

namespace ShortLink.Application.DTOs.IpResponse;

public class IpApiResponse
{
    public string status { get; set; } = string.Empty;
    public string country { get; set; } = string.Empty;
}
