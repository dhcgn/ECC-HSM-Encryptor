using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract;
using NUnit.Framework;

namespace Storage.Test
{
    [TestFixture]
    public class LocalStorageManagerTest
    {
        [SetUp]
        public void Setup()
        {
            File.Delete(LocalStorageManager.StoragePath);
        }

        [Test]
        public void Test()
        {
            new LocalStorageManager("qwert").Add(new EcKeyPair());
            var result = new LocalStorageManager("qwert").GetAll<EcKeyPair>();

            Assert.That(result.Count(), Is.EqualTo(2));
        }
    }
}
