using System;
using System.IO;
using System.Linq;
using Contract;
using ProtoBuf;

namespace Encryption
{
    public partial class HybridEncryption
    {
        public static void Encrypt(Stream input, Stream output, params EcKeyPair[] publicKeys)
        {
            var secretKey = Random.CreateData(256);

            var hybridFileInfo = HybridFileInfo.Create(publicKeys, secretKey);
            var hybridContainerData = hybridFileInfo.ToWire();

            new MemoryStream(hybridContainerData).CopyTo(output);
            SymmetricEncryption.Encrypt(input, output, secretKey);
        }

        public static void Decrypt(Stream input, Stream output, EcKeyPair privateKey)
        {
            var hybridFileInfo = HybridFileInfo.FromWire(input);
            var secretKey = GetSecretKey(privateKey, hybridFileInfo);

            SymmetricEncryption.Decrypt(input, output, secretKey);
        }

        private static byte[] GetSecretKey(EcKeyPair privateKey, HybridFileInfo hybridFileInfo)
        {
            var derivedSecret = hybridFileInfo.DerivedSecrets.FirstOrDefault(secret => secret.PublicKey.ToAns1().SequenceEqual(privateKey.ToAns1()));

            var ds = EllipticCurveCryptographer.DeriveSecret(privateKey, hybridFileInfo.EphemeralKey);

            var derivedSecretInputStream = new MemoryStream(derivedSecret.EncryptedSharedSecret);
            var derivedSecretOutputStream = new MemoryStream();

            SymmetricEncryption.Decrypt(derivedSecretInputStream, derivedSecretOutputStream, ds);

            var secretKey = derivedSecretOutputStream.ToArray();
            return secretKey;
        }
    }
}