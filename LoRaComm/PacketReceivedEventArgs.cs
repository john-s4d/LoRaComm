using System.Net;


    public class PacketReceivedEventArgs
    {
        public IPEndPoint EndPoint { get; set; }
        public Packet Packet { get; set; }
        public PacketReceivedEventArgs(Packet packet, IPEndPoint endPoint)
        {
            Packet = packet;
            EndPoint = endPoint;
        }
    }
