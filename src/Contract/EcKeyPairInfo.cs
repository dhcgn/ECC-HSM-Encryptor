namespace Contract
{
    public class EcKeyPairInfo
    {
        public byte[] ECParamsData { get; set; }
        public string CurveDescription { get; set; }
        public string ManufacturerId { get; set; }
        public string TokenLabel { get; set; }
        public EcKeyPair PublicKey { get; set; }
        public EcIdentifier EcIdentifier { get; set; }

        public string DisplayName => $"Name: '{this.EcIdentifier?.KeyLabel}' Token: '{TokenLabel}' SN: {EcIdentifier?.TokenSerialNumber}";
    }
}