using ProtoBuf;

namespace Contract
{
    [ProtoContract]
    public class EncryptedData : ProtoBase<EncryptedData>
    {
        [ProtoMember(1)]
        public byte[] Iv { get; set; }

        [ProtoMember(2)]
        public byte[] Data { get; set; }

        public int Version { get; set; }
    }
}