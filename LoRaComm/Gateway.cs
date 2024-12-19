using System.Net;


    public enum TxPowerMode
    {   
        RANDOM,
        MINIMUM,
        MAXIMUM,
        MATCH
    }

    public class Gateway
    {
        public string? Id { get; set; }
        public IPEndPoint? EndPoint { get; set; }        
        public TxPowerMode PowerMode { get; set; } = TxPowerMode.MINIMUM;
        public uint MinPower { get; set; } = 12;
        public uint MaxPower { get; set; } = 27;       
        public List<float> TxFrequencies { get; private set; } = new List<float>();

        internal uint GetPower()
        {
            switch(PowerMode)
            {
                case TxPowerMode.MINIMUM:
                    return MinPower;

                case TxPowerMode.MAXIMUM:
                    return MaxPower;                    

                case TxPowerMode.RANDOM:
                    return Utils.RandomUint(MinPower, MaxPower);

                case TxPowerMode.MATCH:
                    throw new NotImplementedException();
            }

            throw new NotSupportedException();
        }

        internal bool CanTxRadio0(float freq)
        {
            return TxFrequencies.Contains(freq);
        }
    }
