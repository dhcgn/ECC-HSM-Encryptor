using ProtoBuf;

namespace Contract
{
    [ProtoContract]
    public class PublicKey : ProtoBase<PublicKey>
    {
        [ProtoMember(1)]
        public byte[] Qx { get; set; }

        [ProtoMember(2)]
        public byte[] Qy { get; set; }

        public int Version { get; set; }
    }
}