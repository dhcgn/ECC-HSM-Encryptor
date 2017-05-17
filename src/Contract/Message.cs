using System;
using System.IO;
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
    }

    [ProtoContract]
    public class Cargo : ProtoBase<Cargo>
    {
        [ProtoMember(1)]
        public EcKeyPair EphemeralEcKeyPair { get; set; }

        [ProtoMember(2)]
        public byte[] HMAC { get; set; }

        [ProtoMember(3)]
        public EncryptedData EncryptedData { get; set; }
    }

    [ProtoContract]
    public class EncryptedData : ProtoBase<EncryptedData>
    {
        [ProtoMember(1)]
        public byte[] Iv { get; set; }

        [ProtoMember(2)]
        public byte[] Data { get; set; }
    }

    [ProtoContract]
    public class EcKeyPair : ProtoBase<EcKeyPair>
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [ProtoMember(1)]
        public byte[] PrivateKey { get; set; }

        [ProtoMember(2)]
        public PublicKey PublicKey { get; set; }
    }

    [ProtoContract]
    public class PublicKey : ProtoBase<PublicKey>
    {
        [ProtoMember(1)]
        public byte[] Qx { get; set; }

        [ProtoMember(2)]
        public byte[] Qy { get; set; }
    }

    [ProtoContract]
    public class ProofOfWork : ProtoBase<ProofOfWork>
    {
        [ProtoMember(1)]
        public int Difficulty { get; set; } = 1;

        [ProtoMember(2)]
        public int Proof { get; set; }
    }

    public class ProtoBase<T>
    {
        [ProtoMember(Int32.MaxValue)]
        public int Version { get; set; }

        public byte[] ToProtoBufData()
        {
            var protoStream = new MemoryStream();
            Serializer.Serialize(protoStream, this);
            return protoStream.ToArray();
        }

        public static T FromProtoBufData(byte[] data)
        {
            var protoStream = new MemoryStream(data);
            return Serializer.Deserialize<T>(protoStream);
        }
    }
}