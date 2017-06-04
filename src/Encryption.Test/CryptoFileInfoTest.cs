using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
}