using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Storage
{
    public class LocalStorageManager
    {
        private readonly DiskStorage diskStorage;

        internal static IStorageProvider storageProvider = new StorageProvider();
        private string password;

        public LocalStorageManager() : this(null)
        {

        }

        public LocalStorageManager(string password)
        {
            this.password = password;

            if (File.Exists(this.StoragePath))
            {
                this.diskStorage = storageProvider.Load(this.StoragePath, password);
            }
            else
            {
                this.diskStorage = new DiskStorage();
            }
        }

        public void RemoveAll<T>()
        {
            var storageType = this.diskStorage.StorageTypes.SingleOrDefault(type => type.Name == typeof(T).Name);
            if (storageType == null)
            {
                return;
            }

            storageType.Entites.Clear();
            storageProvider.Save(this.diskStorage, this.StoragePath, this.password);
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

            storageProvider.Save(this.diskStorage, this.StoragePath, this.password);
        }

        public void AddRange<T>(IEnumerable<T> entities)
        {
            // BUG Performance nightmare
            foreach (var entity in entities)
            {
                this.Add(entity);
            }
        }

        public string StoragePath
        {
            get
            {
                var isEncrypted = this.password != null;
                return GetStoragePath(isEncrypted);
            }
        }

        internal static string GetStoragePath(bool isEncrypted)
        {
            string uri = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            var path = new Uri(uri).LocalPath;
            var directory = Path.GetDirectoryName(path);
            var filename = isEncrypted ? "storage.bin" : "storage.json";
            var storagePath = Path.Combine(directory, filename);
            return storagePath;
        }


    }
}