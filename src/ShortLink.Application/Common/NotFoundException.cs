
namespace ShortLink.Application.Common;

public class NotFoundException : Exception
{
    public NotFoundException()
        : base("The requested resource was not found.")
    {
    }

    // Constructor that accepts a custom message
    public NotFoundException(string message)
        : base(message)
    {
    }
}
