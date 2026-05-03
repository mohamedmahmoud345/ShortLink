using System;

namespace ShortLink.Api.DTOs.ShortUrl;

public record UpdateDto(Guid Id, string Url);