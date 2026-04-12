
namespace ShortLink.Domain.Entities;

public class User
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<ShortLink> Linkes { get; set; }
}
