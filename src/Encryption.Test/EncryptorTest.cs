using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Encryption.Test
{
    [TestFixture]
    public class CryptoFileInfoTest : TestBase
    {

        [Test]
        public void WriteToDisk_FileStream()
        {
            var data = Guid.NewGuid().ToByteArray();
            File.WriteAllBytes(this.InputFile, data);

            var cryptoFileInfo = new CryptoFileInfo()
            {
                Iterations = Int32.MaxValue,
                Version = Int32.MaxValue,
                Iv = RandomHelper.GetRandomData(16),
                Salt = RandomHelper.GetRandomData(16),
            };

            using (var rawFileStream = File.OpenRead(this.InputFile))
            using (var output = File.Create(this.OutputFile))
            {
                CryptoFileInfo.WriteToDisk(cryptoFileInfo, output, rawFileStream);
            }

            Console.Out.WriteLine(nameof(this.OutputFile));
            var outFileData = File.ReadAllBytes(this.OutputFile);
            Console.Out.WriteLine(BitConverter.ToString(outFileData));

            Assert.That(File.Exists(this.OutputFile), Is.True);

            var magicNumber = CryptoFileInfo.MagicNumber;
            Assert.That(outFileData.Take(magicNumber.Count), Is.EquivalentTo(magicNumber));

            var lenght = cryptoFileInfo.ToProtoBufData().Length;
            Assert.That(outFileData.Skip(magicNumber.Count).Take(4), Is.EquivalentTo(BitConverter.GetBytes(lenght)));
        }

        [Test]
        public void WriteToDiskTest_MemoryStream()
        {
            var data = Guid.NewGuid().ToByteArray();
            var inputStream = new MemoryStream(data);

            var cryptoFileInfo = new CryptoFileInfo()
            {
                Iterations = Int32.MaxValue,
                Version = Int32.MaxValue,
                Iv = RandomHelper.GetRandomData(16),
                Salt = RandomHelper.GetRandomData(16),
            };

            using (var output = File.Create(this.OutputFile))
            {
                CryptoFileInfo.WriteToDisk(cryptoFileInfo, output, inputStream);
            }

            Console.Out.WriteLine(nameof(this.OutputFile));
            var outFileData = File.ReadAllBytes(this.OutputFile);
            Console.Out.WriteLine(BitConverter.ToString(outFileData));

            Assert.That(File.Exists(this.OutputFile), Is.True);

            var magicNumber = CryptoFileInfo.MagicNumber;
            Assert.That(outFileData.Take(magicNumber.Count), Is.EquivalentTo(magicNumber));

            var lenght = cryptoFileInfo.ToProtoBufData().Length;
            Assert.That(outFileData.Skip(magicNumber.Count).Take(4), Is.EquivalentTo(BitConverter.GetBytes(lenght)));
        }

        [Test]
        public void WriteToDiskLoadFromDiskTest()
        {
            var data = Guid.NewGuid().ToByteArray();
            File.WriteAllBytes(this.InputFile, data);

            var cryptoFileInfo = new CryptoFileInfo()
            {
                Iterations = Int32.MaxValue,
                Version = Int32.MaxValue,
                Iv = RandomHelper.GetRandomData(16),
                Salt = RandomHelper.GetRandomData(16),
            };

            using (var rawFileStream = File.OpenRead(this.InputFile))
            using (var output = File.Create(this.OutputFile))
            {
                CryptoFileInfo.WriteToDisk(cryptoFileInfo, output, rawFileStream);
            }

            Console.Out.WriteLine(nameof(this.OutputFile));
            Console.Out.WriteLine(BitConverter.ToString(File.ReadAllBytes(this.OutputFile)));

            CryptoFileInfo result;
            using (var raw = File.Create(this.RawFile))
            using (var input = File.OpenRead(this.OutputFile))
            {
                result = CryptoFileInfo.LoadFromDisk(input, raw);
            }

            Assert.That(File.Exists(this.RawFile), Is.True);
            Assert.That(result, Is.Not.Null);
            Assert.That(JsonConvert.SerializeObject(result), Is.EqualTo(JsonConvert.SerializeObject(cryptoFileInfo)));
        }
        [Test]
        public void WriteToDiskLoadFromDiskTest_MemoryStream()
        {
            var data = Guid.NewGuid().ToByteArray();
            var inputStream = new MemoryStream(data);

            var cryptoFileInfo = new CryptoFileInfo()
            {
                Iterations = Int32.MaxValue,
                Version = Int32.MaxValue,
                Iv = RandomHelper.GetRandomData(16),
                Salt = RandomHelper.GetRandomData(16),
            };

            using (var output = File.Create(this.OutputFile))
            {
                CryptoFileInfo.WriteToDisk(cryptoFileInfo, output, inputStream);
            }

            Console.Out.WriteLine(nameof(this.OutputFile));
            Console.Out.WriteLine(BitConverter.ToString(File.ReadAllBytes(this.OutputFile)));

            CryptoFileInfo result;
            using (var raw = File.Create(this.RawFile))
            using (var input = File.OpenRead(this.OutputFile))
            {
                result = CryptoFileInfo.LoadFromDisk(input, raw);
            }

            Assert.That(File.Exists(this.RawFile), Is.True);
            Assert.That(result, Is.Not.Null);
            Assert.That(JsonConvert.SerializeObject(result), Is.EqualTo(JsonConvert.SerializeObject(cryptoFileInfo)));
        }
    }

    [TestFixture]
    public class EncryptorTest :TestBase
    {
      

        [Test]
        public void GetRandomDataTest()
        {
            var bits = 128;
            var result = RandomHelper.GetRandomData(bits);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Length.EqualTo(bits / 8));
        }

        [Test]
        public void EncryptTest()
        {
            var data = Guid.NewGuid().ToByteArray();
            File.WriteAllBytes(this.InputFile, data);

            var pwd = Guid.NewGuid().ToString();

            using (var fileStream = File.OpenRead(this.InputFile))
            using (var output = File.Create(this.OutputFile))
            {
                SymmetricEncryption.Encrypt(fileStream, output, pwd);
            }

            Assert.That(File.ReadAllBytes(this.OutputFile).Length, Is.GreaterThan(data.Length));
        }

        [Test]
        public void EncryptDecryptTest()
        {
            var data = Guid.NewGuid().ToByteArray();
            File.WriteAllBytes(this.InputFile, data);

            var pwd = Guid.NewGuid().ToString();

            using (var input = File.OpenRead(this.InputFile))
            using (var output = File.Create(this.OutputFile))
            {
                SymmetricEncryption.Encrypt(input, output, pwd);
            }

            using (var input = File.OpenRead(this.OutputFile))
            using (var output = File.Create(this.ResultFile))
            {
                SymmetricEncryption.Decrypt(input, output, pwd);
            }

            Assert.That(data, Is.EquivalentTo(File.ReadAllBytes(this.ResultFile)));
        }

    }

    public class TestBase
    {
        internal string InputFile;
        internal string OutputFile;
        internal string ResultFile;
        internal string RawFile;

        [SetUp]
        public void Setup()
        {
            this.InputFile = Path.GetTempFileName();
            this.OutputFile = Path.GetTempFileName();
            this.ResultFile = Path.GetTempFileName();
            this.RawFile = Path.GetTempFileName();
        }


        [TearDown]
        public void TearDown()
        {
            foreach (var file in new[] { this.InputFile, this.OutputFile, this.ResultFile, this.RawFile })
            {
                try
                {
                    if (File.Exists(file))
                        File.Delete(file);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}