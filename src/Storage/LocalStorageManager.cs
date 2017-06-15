using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Storage
{
    public class LocalStorageManager
    {
        private readonly DiskStorage diskStorage;

        internal static IStorageProvider StorageProvider = new StorageProvider();
        private readonly string password;

        public LocalStorageManager() : this(null)
        {
        }

        public LocalStorageManager(string password)
        {
            this.password = password;

            if (File.Exists(this.StoragePath))
            {
                this.diskStorage = StorageProvider.Load(this.StoragePath, password);
            }
            else
            {
                this.diskStorage = new DiskStorage();
            }
        }

        public void RemoveAll(string name)
        {
            var storageType = this.diskStorage.StorageTypes.SingleOrDefault(type => type.Name == name);
            if (storageType == null)
            {
                return;
            }

            storageType.Entites.Clear();
            StorageProvider.Save(this.diskStorage, this.StoragePath, this.password);
        }

        public IEnumerable<T> GetAll<T>(string name)
        {
            var storageType = this.diskStorage.StorageTypes.SingleOrDefault(type => type.Name == name);
            if (storageType == null)
            {
                return Enumerable.Empty<T>();
            }
            return storageType.Entites.Select(o => o.ToObject<T>());
        }

        public void Add<T>(T entity, string name) where T : class, new()
        {
            var storageType = this.diskStorage.StorageTypes.SingleOrDefault(type => type.Name == name);
            if (storageType == null)
            {
                storageType = new StorageType {Name = name};
                this.diskStorage.StorageTypes.Add(storageType);
            }

            storageType.Entites.Add(JObject.FromObject(entity));

            StorageProvider.Save(this.diskStorage, this.StoragePath, this.password);
        }

        public void AddRange<T>(IEnumerable<T> entities, string name) where T : class, new()
        {
            // BUG Performance nightmare
            foreach (var entity in entities)
            {
                this.Add(entity, name);
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