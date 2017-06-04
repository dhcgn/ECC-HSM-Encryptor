using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Storage.Test
{
    [TestFixture]
    public class LocalStorageManagerTest
    {
        [SetUp]
        public void Setup()
        {
            var tryDelete = new Action<string>(s =>
            {
                if(File.Exists(s))
                    File.Delete(s);
            });

            tryDelete(LocalStorageManager.GetStoragePath(false));
            tryDelete(LocalStorageManager.GetStoragePath(true));
        }

        [Test]
        public void SaveAndLoad()
        {
            new LocalStorageManager("qwert").Add(new EcKeyPair());
            var result = new LocalStorageManager("qwert").GetAll<EcKeyPair>();

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public void IsNotJson()
        {
            var localStorageManager = new LocalStorageManager("qwert");
            localStorageManager.Add(new EcKeyPair());

            var content = File.ReadAllText(localStorageManager.StoragePath);

            Assert.Throws<Newtonsoft.Json.JsonReaderException>(() => JsonConvert.DeserializeObject(content),"Can parse as json");
        }

        [Test]
        public void IsJson()
        {
            var localStorageManager = new LocalStorageManager();
            localStorageManager.Add(new EcKeyPair());

            var content = File.ReadAllText(localStorageManager.StoragePath);

            Assert.DoesNotThrow(() => JsonConvert.DeserializeObject(content),"Can parse as json");
        }
    }
}
