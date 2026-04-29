
namespace ShortLink.Application.Features.ShortUrl.Commands.CreateShortUrl.GenerateCode;

public class Base62
{
    private const string Alphabet = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string Encode(Guid guid)
    {
        var bytes = guid.ToByteArray();
        return EncodeBytes(bytes);
    }

    public static Guid Decode(string code)
    {
        var bytes = DecodeBytes(code, 16);
        return new Guid(bytes);
    }

    private static string EncodeBytes(byte[] data)
    {
        var value = new System.Numerics.BigInteger(data.Concat(new byte[] { 0 }).ToArray());
        if (value == 0) return Alphabet[0].ToString();

        var chars = new Stack<char>();
        while (value > 0)
        {
            var remainder = (int)(value % 62);
            chars.Push(Alphabet[remainder]);
            value /= 62;
        }
        return new string(chars.ToArray());
    }

    private static byte[] DecodeBytes(string code, int length)
    {
        System.Numerics.BigInteger result = 0;
        foreach (var ch in code)
        {
            var index = Alphabet.IndexOf(ch);
            if (index < 0) throw new FormatException("Invalid Base62 character.");
            result = result * 62 + index;
        }

        var bytes = result.ToByteArray();
        if (bytes.Length > length)
            bytes = bytes.Take(length).ToArray();
        else if (bytes.Length < length)
            bytes = bytes.Concat(new byte[length - bytes.Length]).ToArray();

        return bytes;
    }
}
