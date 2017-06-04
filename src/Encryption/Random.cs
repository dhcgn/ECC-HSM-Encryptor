using System.Security.Cryptography;

namespace Encryption
{
    public class Random
    {
        public static byte[] CreateSalt(int bit = 512)
        {
            return CreateData(bit);
        }

        public static byte[] CreateData(int bit)
        {
            var salt = new byte[bit / 8];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(salt);
            }
            return salt;
        }
    }
}