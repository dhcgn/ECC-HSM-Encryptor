using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Contract;
using ProtoBuf;

namespace Encryption
{
    [ProtoContract]
    public class DerivedSecret
    {
        [ProtoMember(1)]
        public EcKeyPair PublicKey { get; set; }

        [ProtoMember(2)]
        public byte[] EncryptedSharedSecret { get; set; }
    }

    [ProtoContract]
    public class HybridFileInfo : ProtoBase<HybridFileInfo>
    {
        [ProtoIgnore] public static readonly IReadOnlyList<byte> MagicNumber = new[] {(byte) 154, (byte) 65, (byte) 243, (byte) 167, (byte) 5, (byte) 63, (byte) 211};

        [ProtoMember(1)]
        public List<DerivedSecret> DerivedSecrets { get; set; }

        [ProtoMember(2)]
        public EcKeyPair EphemeralKey { get; set; }

        public byte[] ToWire()
        {
            var hybridData = this.ToProtoBufData();

            var result = new MemoryStream();
            new MemoryStream(MagicNumber.ToArray()).CopyTo(result);
            new MemoryStream(BitConverter.GetBytes(hybridData.Length)).CopyTo(result);
            new MemoryStream(hybridData).CopyTo(result);
            return result.ToArray();
        }

        public static HybridFileInfo FromWire(Stream input)
        {
            byte[] magicNumberdata = new byte[MagicNumber.Count];
            input.Read(magicNumberdata, 0, magicNumberdata.Length);

            byte[] intData = new byte[4];
            input.Read(intData, 0, intData.Length);
            int intValue = BitConverter.ToInt32(intData, 0);

            var protoDate = new byte[intValue];
            input.Read(protoDate, 0, intValue);

            return HybridFileInfo.FromProtoBufData(protoDate);
        }

        public static HybridFileInfo Create(EcKeyPair[] publicKeys, byte[] secretKey)
        {
            var ephemeralKey = EllipticCurveCryptographer.CreateKeyPair(true);

            var result = new HybridFileInfo()
            {
                EphemeralKey = ephemeralKey.ExportPublicKey(),
            };

            result.DerivedSecrets = new List<DerivedSecret>();
            foreach (var publicKey in publicKeys)
            {
                var deriveSecret = EllipticCurveCryptographer.DeriveSecret(ephemeralKey, publicKey);

                var input = new MemoryStream(secretKey);
                var output = new MemoryStream();
                SymmetricEncryption.Encrypt(input, output, deriveSecret);

                var derivedSecret = new DerivedSecret()
                {
                    PublicKey = publicKey.ExportPublicKey(),
                    EncryptedSharedSecret = output.ToArray()
                };
                result.DerivedSecrets.Add(derivedSecret);
            }
            return result;
        }
    }
}