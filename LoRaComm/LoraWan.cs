using System.Net;

public class LoraWan
{
    public delegate void PacketReceivedEventHandler(object sender, PacketReceivedEventArgs e);
    public event PacketReceivedEventHandler PacketReceivedEvent;

    public int UdpPort { get; set; } = 1700;

    private ILogger _logger;

    private UdpClient _udpClient;

    public LoraWan(ILogger logger)
    {
        _logger = logger;
        _udpClient = new UdpClient(UdpPort, _logger);
        _udpClient.DataReceivedEvent += DataReceivedEventHandler;
    }

    private void DataReceivedEventHandler(object sender, DataReceivedEventArgs e)
    {
        Packet packet = new Packet().FromByteArray(e.Data);

        _logger.Write(this, LogLevel.FINEST, $"{e.EndPoint.Address}:{e.EndPoint.Port} | {packet.Token} | data received");

        PacketReceivedEvent?.Invoke(this, new PacketReceivedEventArgs(packet, e.EndPoint));


    }

    public void Send(Packet packet, IPEndPoint endPoint)
    {
        _udpClient.Send(packet.ToByteArray(), endPoint);
        _logger.Write(this, LogLevel.FINEST, $"{endPoint.Address}:{endPoint.Port} | {packet.Token} | data sent");
    }

    public void Start()
    {
        _udpClient?.Start();
    }

    public void Stop()
    {
        _udpClient?.Stop();
    }
}
