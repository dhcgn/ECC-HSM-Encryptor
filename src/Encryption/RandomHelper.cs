using System.Security.Cryptography;

namespace Encryption
{
    public class RandomHelper
    {
        public static byte[] GetRandomData(int bits)
        {
            var result = new byte[bits / 8];
            using (var randomNumberGenerator = RandomNumberGenerator.Create())
            {
                randomNumberGenerator.GetBytes(result);
            }
            return result;
        }
    }
}