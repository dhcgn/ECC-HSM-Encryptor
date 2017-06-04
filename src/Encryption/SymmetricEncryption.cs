using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Encryption
{
    public class SymmetricEncryption
    {
        public static void Decrypt(Stream input, Stream output, byte[] secretKey)
        {
            DecryptInternal(input, output, null, secretKey.Take(256/8).ToArray());
        }

        public static void Decrypt(Stream input, Stream output, string password)
        {
            DecryptInternal(input, output, password, null);
        }

        private static void DecryptInternal(Stream input, Stream output, string password, byte[] secretKey)
        {
            var tempPath = Path.GetTempFileName();
            CryptoFileInfo cryptoFileInfo;
            using (var rawfile = File.Create(tempPath))
            {
                cryptoFileInfo = CryptoFileInfo.LoadFromDisk(input, rawfile);
            }

            var keyAes = password != null 
                ? CreateAesKeyFromPassword(password, cryptoFileInfo.Salt, cryptoFileInfo.Iterations) 
                : secretKey;

            using (var tempFile = File.OpenRead(tempPath))
            using (var aes = Aes.Create())
            {
                aes.Key = keyAes;
                aes.IV = cryptoFileInfo.Iv;

                using (var encryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    using (var aesStream = new CryptoStream(tempFile, encryptor, CryptoStreamMode.Read))
                    {
                        aesStream.CopyTo(output);
                    }
                }
            }

            File.Delete(tempPath);
        }

        public static void Encrypt(Stream input, Stream output, string password)
        {
#if DEBUG
            var iterations = 100;
#else
            var iterations = 100000;
#endif
            var salt = RandomHelper.GetRandomData(128);
            var keyAes = CreateAesKeyFromPassword(password, salt, iterations);

            var iv = RandomHelper.GetRandomData(128);

            EncryptInternal(input, output, keyAes,iv, salt, iterations);
        }

        public static void Encrypt(Stream input, Stream output, byte[] secretKey)
        {
            var salt = RandomHelper.GetRandomData(128);
            var keyAes = secretKey.Take(256 / 8).ToArray();

            var iv = RandomHelper.GetRandomData(128);

            EncryptInternal(input, output, keyAes, iv, salt, 0);
        }

        public static void EncryptInternal(Stream input, Stream output, byte[] keyAes, byte[] iv, byte[] salt, int iterations)
        {
            var tempPath = Path.GetTempFileName();
            using (var tempFile = File.Create(tempPath))
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = keyAes;
                    aes.IV = iv;

                    using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                    {
                        using (var aesStream = new CryptoStream(tempFile, encryptor, CryptoStreamMode.Write))
                        {
                            input.CopyTo(aesStream);
                        }
                    }
                }
            }

            var cryptoFileInfo = new CryptoFileInfo
            {
                Iv = iv,
                Salt = salt,
                Iterations = iterations,
            };

            using (var tempStream = File.OpenRead(tempPath))
            {
                CryptoFileInfo.WriteToDisk(cryptoFileInfo, output, tempStream);
            }
            File.Delete(tempPath);
        }

        private static byte[] CreateAesKeyFromPassword(string password, byte[] salt, int iterations)
        {
            byte[] keyAes;
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                keyAes = deriveBytes.GetBytes(256 / 8);
            }
            return keyAes;
        }


    }
}