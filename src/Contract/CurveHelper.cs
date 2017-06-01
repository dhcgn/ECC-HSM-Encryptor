using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Contract
{
    public static class CurveHelper
    {
        private static readonly List<Tuple<string, string, string, int>> Mapping = new List<Tuple<string, string, string, int>>()
        {
            Tuple.Create("secp192r1",       "1.2.840.10045.3.1.1", "06082A8648CE3D030101", 192),
            Tuple.Create("prime192v1",      "1.2.840.10045.3.1.1", "06082A8648CE3D030101", 192),
            Tuple.Create("nistp192",        "1.2.840.10045.3.1.1", "06082A8648CE3D030101", 192),
            Tuple.Create("ansiX9p192r1",    "1.2.840.10045.3.1.1", "06082A8648CE3D030101", 192),

            Tuple.Create("secp224r1",       "1.3.132.0.33", "06052b81040021", 224),
            Tuple.Create("nistp224",        "1.3.132.0.33", "06052b81040021", 224),

            Tuple.Create("secp256r1",       "1.2.840.10045.3.1.7", "06082A8648CE3D030107", 256),
            Tuple.Create("prime256v1",      "1.2.840.10045.3.1.7", "06082A8648CE3D030107", 256),
            Tuple.Create("nistp256",        "1.2.840.10045.3.1.7", "06082A8648CE3D030107", 256),
            Tuple.Create("ansiX9p256r1",    "1.2.840.10045.3.1.7", "06082A8648CE3D030107", 256),

            Tuple.Create("secp384r1",       "1.3.132.0.34", "06052B81040022", 384),
            Tuple.Create("prime384v1",      "1.3.132.0.34", "06052B81040022", 384),
            Tuple.Create("nistp384",        "1.3.132.0.34", "06052B81040022", 384),
            Tuple.Create("ansiX9p384r1",    "1.3.132.0.34", "06052B81040022", 384),

            Tuple.Create("secp521r1",       "1.3.132.0.35", "06052B81040023", 521),
            Tuple.Create("nistp521",        "1.3.132.0.35", "06052B81040023", 521),

            Tuple.Create("brainpoolP192r1", "1.3.36.3.3.2.8.1.1.3", "06092B2403030208010103", 192),
            Tuple.Create("brainpoolP224r1", "1.3.36.3.3.2.8.1.1.5", "06092B2403030208010105", 224),
            Tuple.Create("brainpoolP256r1", "1.3.36.3.3.2.8.1.1.7", "06092B2403030208010107", 256),
            Tuple.Create("brainpoolP320r1", "1.3.36.3.3.2.8.1.1.9", "06092B2403030208010109", 320),

            Tuple.Create("secp192k1",       "1.3.132.0.31", "06052B8104001F", 192),
            Tuple.Create("secp256k1",       "1.3.132.0.10", "06052B8104000A", 256),
        };
        public static string GetCurveDescriptionFromEcParam(byte[] ecParam)
        {
            return GetCurveDescriptionFromEcParam(BitConverter.ToString(ecParam).Replace("-", null));
        }

        public static string GetCurveDescriptionFromEcParam(string ecParam)
        {
            var value =  Mapping.SingleOrDefault(tuple => tuple.Item3.ToLower() == ecParam.ToLower());
            if(value==null)
                return $"Unkown ({ecParam})";

            return $"{value.Item1} ({value.Item4} bit)";
        }
    }
}
