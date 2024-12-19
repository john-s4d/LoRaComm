using System.Text.Json.Serialization;


    public class StatPacket
    {
        public string time { get; set; }
        public float lati { get; set; }
        [JsonPropertyName("long")]
        public float longi { get; set; }
        public int alti { get; set; }
        public uint rxnb { get; set; }
        public int rxok { get; set; }
        public uint rxfw { get; set; }
        public float ackr { get; set; }
        public uint dwnb { get; set; }
        public uint txnb { get; set; }
        public string pfrm { get; set; }
        public string mail { get; set; }
        public string desc { get; set; }
    }
