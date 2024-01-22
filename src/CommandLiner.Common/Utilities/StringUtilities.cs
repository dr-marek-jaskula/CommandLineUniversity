using System.Security;
using System.Text;

namespace CommandLiner.Common.Utilities;

public static class StringUtilities
{
    public static Stream ToStream(this string input)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(input));
    }

    public static byte[] ToBytes(this string input)
    {
        return Encoding.UTF8.GetBytes(input);
    }

    public static SecureString ToSecureString(this string input)
    {
        var secureString = new SecureString();

        foreach (var character in input)
        {
            secureString.AppendChar(character);
        }

        secureString.MakeReadOnly();

        return secureString;
    }
}