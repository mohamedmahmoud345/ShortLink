namespace ShortLink.Api.DTOs.ClickEvent;

public record class RecordDto
(
    string ShortCode,
    string Referrer,
    string IpAddress,
    string UserAgent
);
