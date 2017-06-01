using System;
using Newtonsoft.Json;
using ProtoBuf;

namespace Contract
{
    [ProtoContract]
    public class Message : ProtoBase<Message>
    {
        [ProtoMember(1)]
        public ProofOfWork ProofOfWork { get; set; }

        [ProtoIgnore]
        public byte[] CargoData
        {
            get => this.Cargo?.ToProtoBufData();
            set => this.Cargo = Cargo.FromProtoBufData(value);
        }

        [JsonIgnore]
        [ProtoMember(2)]
        public Cargo Cargo { get; set; }

        public int Version { get; set; }
    }
}