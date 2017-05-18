using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Storage
{
    public class StorageProvider: IStorageProvider
    {
        public DiskStorage Load(string storagePath, string password)
        {
            string jsonData;

            using (var output = new MemoryStream())
            using (var input = File.OpenRead(storagePath))
            {
                Encryption.SymmetricEncryption.Decrypt(input, output, password);
                jsonData = Encoding.UTF8.GetString(output.ToArray());
            }

            return JsonConvert.DeserializeObject<DiskStorage>(jsonData);

        }

        public void Save(DiskStorage diskStorage, string storagePath, string password)
        {
            var json = JsonConvert.SerializeObject(diskStorage);
            var jsonData = Encoding.UTF8.GetBytes(json);

            using (var input = new MemoryStream(jsonData))
            using (var output = File.OpenWrite(storagePath))
            {
                Encryption.SymmetricEncryption.Encrypt(input, output, password);
            }
        }
    }

    public interface IStorageProvider
    {
        DiskStorage Load(string storagePath, string password);

        void Save(DiskStorage diskStorage, string storagePath, string password);
    }

    public class LocalStorageManager
    {
        private readonly DiskStorage diskStorage;

        internal static IStorageProvider storageProvider = new StorageProvider();
        private string password;

        public LocalStorageManager(string password)
        {
            this.password = password;

            if (File.Exists(StoragePath))
            {
                this.diskStorage = storageProvider.Load(StoragePath, password);
            }
            else
            {
                diskStorage = new DiskStorage();
            }
        }

        public IEnumerable<T> GetAll<T>()
        {
            var storageType = this.diskStorage.StorageTypes.SingleOrDefault(type => type.Name == typeof(T).Name);
            if (storageType == null)
            {
                return Enumerable.Empty<T>();
            }
            return storageType.Entites.Select(o => JsonConvert.DeserializeObject<T>(o));
        }

        public void Add<T>(T entity)
        {
            var storageType = this.diskStorage.StorageTypes.SingleOrDefault(type => type.Name == typeof(T).Name);
            if (storageType == null)
            {
                storageType = new StorageType() {Name = typeof(T).Name};
                this.diskStorage.StorageTypes.Add(storageType);
            }

            storageType.Entites.Add(JsonConvert.SerializeObject(entity));

            storageProvider.Save(this.diskStorage, StoragePath, this.password);
        }



        public static string StoragePath
        {
            get
            {
                string uri = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
                var path = new Uri(uri).LocalPath;
                var directory = System.IO.Path.GetDirectoryName(path);
                var storagePath = Path.Combine(directory, "storage.json");
                return storagePath;
            }
        }
    }

    public class DiskStorage
    {
        public DiskStorage()
        {
            this.StorageTypes = new List<StorageType>();
        }

        public List<StorageType> StorageTypes { get; set; }
    }

    public class StorageType
    {
        public StorageType()
        {
            this.Entites = new List<string>();
        }

        public string Name { get; set; }
        public List<string> Entites { get; set; }
    }
}