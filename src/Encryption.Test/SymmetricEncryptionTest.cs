using System;
using System.IO;
using NUnit.Framework;

namespace Encryption.Test
{
    [TestFixture]
    public class SymmetricEncryptionTest : TestBase
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
}