using System.Collections.Generic;


    public class RxPacket
    {
        public IEnumerable<RxPk> rxpk { get; set; }
        public StatPacket stat { get; set; }
    }
    public class RxPk
    {
        public int jver { get; set; }
        public string time { get; set; }
        public int tmns { get; set; }
        public uint tmst { get; set; }
        public float freq { get; set; }
        public uint chan { get; set; }
        public uint rfch { get; set; }
        public int mid { get; set; }
        public int stat { get; set; }
        public string modu { get; set; }
        public string datr { get; set; }
        public string codr { get; set; }
        public int rssis { get; set; }
        public int rssi { get; set; }
        public float lsnr { get; set; }
        public int foff { get; set; }
        public uint size { get; set; }
        public string data { get; set; }
    }
