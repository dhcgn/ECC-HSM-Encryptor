using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Encryption
{
    public partial class SymmetricEncryption
    {
        public static void Decrypt(Stream input, Stream output, string password)
        {
            var tempPath = Path.GetTempFileName();
            CryptoFileInfo cryptoFileInfo;
            using (var rawfile = File.Create(tempPath))
            {
                cryptoFileInfo = CryptoFileInfo.LoadFromDisk(input, rawfile);
            }

            var keyAes = CreateAesKeyFromPassword(password, cryptoFileInfo.Salt, cryptoFileInfo.Iterations);

            using(var tempFile = File.OpenRead(tempPath))
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

            using (var tempStream =  File.OpenRead(tempPath))
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