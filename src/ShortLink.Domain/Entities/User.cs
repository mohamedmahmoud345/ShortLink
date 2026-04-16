namespace ShortLink.Domain.Entities;

public class User 
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime CreatedAt { get; set; }

    public ICollection<ShortUrl> Links { get; set; }
}
