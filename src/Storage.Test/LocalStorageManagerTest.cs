using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EncryptionSuite.Contract;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Storage.Test
{
    [TestFixture]
    public class LocalStorageManagerTest
    {
        private string storageName1;
        private string storageName2;
        private string storageName3;

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

            this.storageName1 = Guid.NewGuid().ToString();
            this.storageName2 = Guid.NewGuid().ToString();
            this.storageName3 = Guid.NewGuid().ToString();
        }

        [Test]
        public void SaveAndLoad_Encrypted()
        {
            new LocalStorageManager("qwert").Add(new EcKeyPair(), this.storageName1);
            var result = new LocalStorageManager("qwert").GetAll<EcKeyPair>(this.storageName1);

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public void SaveAndLoad()
        {
            new LocalStorageManager().Add(new EcKeyPair(), this.storageName1);

            Console.Out.WriteLine(File.ReadAllText(LocalStorageManager.GetStoragePath(false)));

            var result = new LocalStorageManager().GetAll<EcKeyPair>(this.storageName1);

            Assert.That(result.Count(), Is.EqualTo(1));
        }

        [Test]
        public void SaveAndLoad_DifferentTypeDifferentValues()
        {
            #region Arrange

            new LocalStorageManager().Add(new EcKeyPair(){Version = 1}, this.storageName1);
            new LocalStorageManager().Add(new EcKeyPair(){Version = 2}, this.storageName1);
            new LocalStorageManager().Add(new EcIdentifier(), this.storageName2);
            new LocalStorageManager().Add(new EcIdentifier(), this.storageName2);
            new LocalStorageManager().Add(new EcIdentifier(), this.storageName2);
            new LocalStorageManager().Add(new EcKeyPairInfo(), this.storageName3);

            Console.Out.WriteLine(File.ReadAllText(LocalStorageManager.GetStoragePath(false)));

            #endregion

            #region Act

            var result1 = new LocalStorageManager().GetAll<EcKeyPair>(this.storageName1);
            var result2 = new LocalStorageManager().GetAll<EcIdentifier>(this.storageName2);
            var result3 = new LocalStorageManager().GetAll<EcKeyPairInfo>(this.storageName3);

            #endregion

            #region Assert

            Assert.That(result1.Count(), Is.EqualTo(2));
            Assert.That(result2.Count(), Is.EqualTo(3));
            Assert.That(result3.Count(), Is.EqualTo(1));

            Assert.That(result1.ToArray()[0].Version, Is.EqualTo(1));
            Assert.That(result1.ToArray()[1].Version, Is.EqualTo(2));

            #endregion
        }

        [Test]
        public void IsJson()
        {
            var localStorageManager = new LocalStorageManager();
            localStorageManager.Add(new EcKeyPair(), this.storageName1);

            var content = File.ReadAllText(localStorageManager.StoragePath);

            Assert.DoesNotThrow(() => JsonConvert.DeserializeObject(content),"Can parse as json");
        }
    }
}
