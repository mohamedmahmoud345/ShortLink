namespace ShortLink.Application.DTOs.Account;

public record class AuthDto(Guid Id, string Email, string Name, bool Succeeded);
