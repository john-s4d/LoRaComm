    public class TxPacket
    {
        public TxPk txpk { get; set; }

        public TxPacket(TxPk txPx)
        {
            txpk = txPx;
        }
    }
    public class TxPk
    {
        public bool imme { get; set; }
        public int tmst { get; set; }
        public int tmms { get; set; }
        public float freq { get; set; }
        public uint rfch { get; set; }
        public uint powe { get; set; }
        public string modu { get; set; }
        public string datr { get; set; }
        //public uint datr { get; set; }
        public string codr { get; set; }
        public uint fdev { get; set; }
        public bool ipol { get; set; }
        public uint prea { get; set; }
        public uint size { get; set; }
        public string data { get; set; }
        public bool ncrc { get; set; }

    }
