using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Contract;

namespace Encryption
{
    public class EllipticCurveCryptographer
    {
        private static ECCurve brainpoolP320R1 = ECCurve.NamedCurves.brainpoolP320r1;

        /// <summary>
        /// brainpoolP320r1 is NitroKey HSM compatible.
        /// </summary>
        public string NamedCurve => brainpoolP320R1.ToString();

        public static EcKeyPair CreateKeyPair(bool includePrivateParameters)
        {
            var ecDsa = ECDsa.Create(brainpoolP320R1);
            var exportParameters = ecDsa.ExportParameters(includePrivateParameters);

            return EcKeyPair.CreateFromECParameters(exportParameters);
        }

        public static byte[] DeriveSecret(EcKeyPair privateKeyPair, EcKeyPair publicKeyPair)
        {
            var dhPrivate = ECDiffieHellman.Create(privateKeyPair.CreateECParameters());
            var dhPublic = ECDiffieHellman.Create(publicKeyPair.CreateECParameters());

            return dhPrivate.DeriveKeyFromHash(dhPublic.PublicKey, HashAlgorithmName.SHA512);
        }

        public static byte[] SignData(EcKeyPair privateKeyPair, byte[] data)
        {
            var ecDsa = ECDsa.Create(brainpoolP320R1);
            ecDsa.ImportParameters(privateKeyPair.CreateECParameters());
            return ecDsa.SignData(data, HashAlgorithmName.SHA512);
        }

        public static bool VerifyData(EcKeyPair signedKeyPair, byte[] data, byte[] signature)
        {
            var ecDsa = ECDsa.Create(brainpoolP320R1);
            ecDsa.ImportParameters(signedKeyPair.CreateECParameters());
            return ecDsa.VerifyData(data, signature, HashAlgorithmName.SHA512);
        }

       
    }


}