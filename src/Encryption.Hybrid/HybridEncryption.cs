using System;
using System.IO;
using System.Linq;
using Contract;

namespace Encryption.Hybrid
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

        public static void Decrypt(Stream input, Stream output, string password)
        {
            var hybridFileInfo = HybridFileInfo.FromWire(input);

            var keys = Encryption.NitroKey.EllipticCurveCryptographer.GetEcKeyPairInfos();

            EcIdentifier ecIdentifier = keys.FirstOrDefault(info => hybridFileInfo.DerivedSecrets.Any(secret => info.PublicKey.ToAns1().SequenceEqual(secret.PublicKey.ToAns1())) )?.EcIdentifier;

            if(ecIdentifier==null)
                throw new Exception("Couldn't find any key on any token");

            var symmetricKey = GetSecretKey(ecIdentifier, hybridFileInfo, password);
            SymmetricEncryption.Decrypt(input, output, symmetricKey);
        }

        private static byte[] GetSecretKey(EcIdentifier ecIdentifier, HybridFileInfo hybridFileInfo, string password)
        {
            var publicKey = Encryption.NitroKey.EllipticCurveCryptographer.GetPublicKey(ecIdentifier, password);
            var derivedSecret = hybridFileInfo.DerivedSecrets.FirstOrDefault(secret => secret.PublicKey.ToAns1().SequenceEqual(publicKey.ToAns1()));
            var ds = Encryption.NitroKey.EllipticCurveCryptographer.DeriveSecret(ecIdentifier, hybridFileInfo.EphemeralKey, password);

            var derivedSecretInputStream = new MemoryStream(derivedSecret.EncryptedSharedSecret);
            var derivedSecretOutputStream = new MemoryStream();

            SymmetricEncryption.Decrypt(derivedSecretInputStream, derivedSecretOutputStream, ds);

            var secretKey = derivedSecretOutputStream.ToArray();
            return secretKey;
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