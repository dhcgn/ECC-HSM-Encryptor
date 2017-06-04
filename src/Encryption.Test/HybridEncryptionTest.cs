using System.IO;
using NUnit.Framework;

namespace Encryption.Test
{
    [TestFixture]
    public class HybridEncryptionTest :  TestBase
    {
        [Test]
        public void EncryptMultipleKeys()
        {
            var alice = EllipticCurveCryptographer.CreateKeyPair(true);
            var bob = EllipticCurveCryptographer.CreateKeyPair(true);
            var guenther = EllipticCurveCryptographer.CreateKeyPair(true);

            using (var input = File.OpenRead(this.InputFile))
            using (var output = File.Create(this.OutputFile))
            {
                HybridEncryption.Encrypt(input, output, alice.ExportPublicKey(), bob.ExportPublicKey(), guenther.ExportPublicKey());
            }
        }

        [Test]
        public void EncryptAndDecrypt()
        {
            var alice = EllipticCurveCryptographer.CreateKeyPair(true);

            using (var input = File.OpenRead(this.InputFile))
            using (var output = File.Create(this.OutputFile))
            {
                HybridEncryption.Encrypt(input, output, alice.ExportPublicKey());
            }

            using (var input = File.OpenRead(this.OutputFile))
            using (var output = File.Create(this.ResultFile))
            {
                HybridEncryption.Decrypt(input, output, alice);
            }

            var inputData = File.ReadAllBytes(this.InputFile);
            var outputData = File.ReadAllBytes(this.OutputFile);
            var resultData = File.ReadAllBytes(this.ResultFile);

            Assert.That(inputData.Length, Is.LessThan(outputData.Length), "Input file is smaller than output file");
            Assert.That(outputData, Is.Not.EquivalentTo(resultData), "Encrypted file is not equal to plain file");
            Assert.That(inputData.Length, Is.EqualTo(resultData.Length), "size of plain file is equal to encrypted file");
            Assert.That(inputData, Is.EquivalentTo(resultData), "plain file is equal to encrypted file");
        }

        [Test]
        public void EncryptAndDecryptMultipleKeys()
        {
            var alice = EllipticCurveCryptographer.CreateKeyPair(true);
            var bob = EllipticCurveCryptographer.CreateKeyPair(true);
            var guenther = EllipticCurveCryptographer.CreateKeyPair(true);

            using (var input = File.OpenRead(this.InputFile))
            using (var output = File.Create(this.OutputFile))
            {
                HybridEncryption.Encrypt(input, output, alice.ExportPublicKey(), bob.ExportPublicKey(), guenther.ExportPublicKey());
            }

            using (var input = File.OpenRead(this.OutputFile))
            using (var output = File.Create(this.ResultFile))
            {
                HybridEncryption.Decrypt(input, output, alice);
            }

            var inputData = File.ReadAllBytes(this.InputFile);
            var outputData = File.ReadAllBytes(this.OutputFile);
            var resultData = File.ReadAllBytes(this.ResultFile);

            Assert.That(inputData.Length, Is.LessThan(outputData.Length), "Input file is smaller than output file");
            Assert.That(outputData, Is.Not.EquivalentTo(resultData), "Encrypted file is not equal to plain file");
            Assert.That(inputData.Length, Is.EqualTo(resultData.Length), "size of plain file is equal to encrypted file");
            Assert.That(inputData, Is.EquivalentTo(resultData), "plain file is equal to encrypted file");
        }
    }
}