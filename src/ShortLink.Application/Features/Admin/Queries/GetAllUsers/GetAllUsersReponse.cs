
namespace ShortLink.Application.Features.Admin.Queries.GetAllUsers;

public class GetAllUsersReponse
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public int NumberOfUrls { get; set; }
    public string Role { get; set; }
}
