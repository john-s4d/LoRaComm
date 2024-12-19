using System;
using System.IO;
using System.Text;


    public enum PacketType : byte
    {
        PUSH_DATA = 0,
        PUSH_ACK = 1,
        PULL_DATA = 2,
        PULL_RESP = 3,
        PULL_ACK = 4,
        TX_ACK = 5
    }

    public class Packet
    {
        public byte Version { get; set; }
        public ushort Token { get; set; }
        public PacketType Type { get; set; }
        public string GatewayId { get; set; }
        public string Json { get; set; }

        public Packet() { }

        public Packet(PacketType type, ushort token)
        {
            Version = 2;
            Type = type;
            Token = token;
        }
                
        public Packet FromByteArray(byte[] data)
        {
            Version = data[0] == 2 ? data[0] : throw new InvalidDataException($"Expected Version 2. Received Version {Version}");

            Token = BitConverter.ToUInt16(data, 1);

            Type = (PacketType)data[3];
            if (Type == PacketType.PULL_ACK || Type == PacketType.PUSH_ACK || Type == PacketType.PULL_RESP)
            {
                throw new NotImplementedException();
            }

            GatewayId = Utils.ToHexString(data, 4, 8);

            if (Type == PacketType.PUSH_DATA || Type == PacketType.TX_ACK && data.Length > 12)
            {
                Json = Encoding.ASCII.GetString(data, 12, data.Length - 12);
            }

            return this;
        }

        public byte[] ToByteArray()
        {
            byte[] data = null;

            if (Type == PacketType.PUSH_DATA || Type == PacketType.TX_ACK || Type == PacketType.PULL_DATA)
            {
                throw new NotImplementedException();
            }

            if (Type == PacketType.PULL_ACK || Type == PacketType.PUSH_ACK)
            {
                data = new byte[4];
            }

            if (Type == PacketType.PULL_RESP)
            {
                byte[] json = Encoding.ASCII.GetBytes(Json);
                data = new byte[4 + json.Length];
                json.CopyTo(data, 4);
            }

            data[0] = 2;
            BitConverter.GetBytes(Token).CopyTo(data, 1);
            data[3] = (byte)Type;

            return data;
        }

        public override string ToString()
        {
            return $"{Version} {Token} {Type} {GatewayId} {Json}";
        }
    }
