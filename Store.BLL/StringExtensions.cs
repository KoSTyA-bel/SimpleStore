using System.Security.Cryptography;
using System.Text;

namespace Store.BLL;

public static class StringExtensions
{
    private static MD5 _hasher = MD5.Create();

    public static byte[] GetMD5Hash(this string str) => _hasher.ComputeHash(Encoding.ASCII.GetBytes(str));
}
