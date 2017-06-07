using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using EncryptionSuite.Contract;

namespace EccHsmEncryptor.Presentation.DesignData
{
    public static class DesignDataFactory
    {
        public static T CreateDesignData<T>(params string[] propertyValues) where T : class
        {
            if (typeof(T).Name.Contains("EcKeyPairInfo"))
            {
                var seed = Encoding.UTF8.GetBytes(propertyValues.Aggregate((s, s1) => s + s1));
                seed = SHA1.Create().ComputeHash(seed);
                var key = ToHexString(seed);

                if (!KeyStorage.ContainsKey(key))
                    KeyStorage.Add(key, EncryptionSuite.Encryption.EllipticCurveCryptographer.CreateKeyPair(false));

                return new EncryptionSuite.Contract.EcKeyPairInfo()
                {
                    TokenLabel = propertyValues[1],
                    ManufacturerId = "Nitrokey",
                    CurveDescription = "brainpoolP320r1 (320 bit)",
                    ECParamsData = StringToByteArray("06092b2403030208010109"),
                    PublicKey = KeyStorage[key],
                    EcIdentifier = new EcIdentifier()
                    {
                        KeyLabel = propertyValues[0],
                        TokenSerialNumber = propertyValues[2]
                    }
                } as T;
            }

            return default(T);
        }

        private static Dictionary<string, EcKeyPair> KeyStorage = new Dictionary<string, EcKeyPair>();

        private static string ToHexString(byte[] data)
        {
            return BitConverter.ToString(data).ToLower().Replace("-", null);
        }

        private static byte[] StringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }
    }
}