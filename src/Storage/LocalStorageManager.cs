using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Storage
{
    public class LocalStorageManager
    {
        private readonly DiskStorage diskStorage;

        public LocalStorageManager(string password)
        {
            if (File.Exists(StoragePath))
            {
                this.diskStorage = JsonConvert.DeserializeObject<DiskStorage>(File.ReadAllText(StoragePath));
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

            Save(this.diskStorage);
        }

        private static void Save(DiskStorage storage)
        {
            var json = JsonConvert.SerializeObject(storage);
            File.WriteAllText(StoragePath, json);
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
            this.StorageTypes=new List<StorageType>();
        }
        public List<StorageType> StorageTypes { get; set; }
    }

    public class StorageType
    {
        public StorageType()
        {
            this.Entites=new List<string>();
        }
        public string Name { get; set; }
        public List<string> Entites { get; set; }
    }
}