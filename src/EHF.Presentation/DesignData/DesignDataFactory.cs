using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contract;
using Newtonsoft.Json;

namespace EHF.Presentation.DesignData
{
    public static class DesignDataFactory
    {
        public static T CreateDesignData<T>() where T : class
        {
            if (typeof(T).Name.Contains("EcKeyPairInfo"))
            {
                return new Contract.EcKeyPairInfo()
                {
                    Label = "Label",
                    TokenLabel = "Tokenlabel",
                    ManufacturerId = "NitroKey",
                    TokenSerialNumber = "31431434",
                    CurveDescription = "brainpoolP320r1 (320 bit)",
                    ECParamsData = StringToByteArray("06092b2403030208010109"),
                    PublicKey = EcKeyPair.CreateFromAnsi(StringToByteArray("045104a03e0974ae3678587b6068f8b6420a01249b943649dc0e698ab2cad7048f5363e6ffa1b36455ca8986888919ca486aec898546448bb5d6abf99cf8036477adeb709f56349aed34afec0dc49a6be3678d"))
                } as T;
            }

            return default(T);
        }

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
