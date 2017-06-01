using ProtoBuf;

namespace Contract
{
    [ProtoContract]
    public class Cargo : ProtoBase<Cargo>
    {
        [ProtoMember(1)]
        public EcKeyPair EphemeralEcKeyPair { get; set; }

        [ProtoMember(2)]
        public byte[] HMAC { get; set; }

        [ProtoMember(3)]
        public EncryptedData EncryptedData { get; set; }

        public int Version { get; set; }
    }
}