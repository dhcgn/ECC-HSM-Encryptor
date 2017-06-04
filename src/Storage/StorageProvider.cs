using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Storage
{
    public class StorageProvider : IStorageProvider
    {
        public DiskStorage Load(string storagePath, string password)
        {
            string jsonData;

            using (var output = new MemoryStream())
            using (var input = File.OpenRead(storagePath))
            {
                if (password == null)
                {
                    jsonData = File.ReadAllText(storagePath);
                }
                else
                {
                    Encryption.SymmetricEncryption.Decrypt(input, output, password);
                    jsonData = Encoding.UTF8.GetString(output.ToArray());
                }
            }

            return JsonConvert.DeserializeObject<DiskStorage>(jsonData);
        }

        public void Save(DiskStorage diskStorage, string storagePath, string password)
        {
            var json = JsonConvert.SerializeObject(diskStorage, Formatting.Indented);
            var jsonData = Encoding.UTF8.GetBytes(json);

            using (var input = new MemoryStream(jsonData))
            using (var output = File.OpenWrite(storagePath))
            {
                if (password == null)
                {
                    input.CopyTo(output);
                }
                else
                {
                    Encryption.SymmetricEncryption.Encrypt(input, output, password);
                }
            }
        }
    }
}