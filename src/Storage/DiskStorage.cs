using System.Collections.Generic;

namespace Storage
{
    public class DiskStorage
    {
        public DiskStorage()
        {
            this.StorageTypes = new List<StorageType>();
        }

        public List<StorageType> StorageTypes { get; set; }
    }
}