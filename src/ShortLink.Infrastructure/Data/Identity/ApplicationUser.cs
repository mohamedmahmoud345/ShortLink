using Microsoft.AspNetCore.Identity;
using ShortLink.Domain.Entities;

namespace ShortLink.Infrastructure.Data.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public ICollection<ShortUrl> Urls { get; set; }
}
