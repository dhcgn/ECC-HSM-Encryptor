namespace Contract
{
    public class EcKeyPairInfo
    {
        public string Label { get; set; }
        public byte[] ECParamsData { get; set; }
        public string CurveDescription { get; set; }
        public string ManufacturerId { get; set; }
        public string TokenLabel { get; set; }
        public string TokenSerialNumber { get; set; }
        public EcKeyPair PublicKey { get; set; }

        public string DisplayName => $"{Label}-{TokenLabel}-{TokenSerialNumber}";
    }
}