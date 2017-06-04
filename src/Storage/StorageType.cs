using System.Collections.Generic;

namespace Storage
{
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