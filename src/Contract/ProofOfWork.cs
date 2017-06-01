using ProtoBuf;

namespace Contract
{
    [ProtoContract]
    public class ProofOfWork : ProtoBase<ProofOfWork>
    {
        [ProtoMember(1)]
        public int Difficulty { get; set; } = 1;

        [ProtoMember(2)]
        public int Proof { get; set; }

        public int Version { get; set; }
    }
}