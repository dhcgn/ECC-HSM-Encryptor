using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Storage
{
    public class StorageType
    {
        public StorageType()
        {
            this.Entites = new List<JObject>();
        }

        public string Name { get; set; }
        public List<JObject> Entites { get; set; }
    }
}