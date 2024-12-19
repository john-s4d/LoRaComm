using System;
using System.Net;
using System.Text.Json;
using System.Threading;

public class LoRaComm
{
    private LoraWan _loraWan;
    private ILogger _logger;
    private Timer _broadcastTimer; // Timer for broadcasting messages
    private Random _random = new Random(); // Random generator for message selection

    private Dictionary<string, Gateway> _gateways = new();
    private List<string> _messages = new();

    public LoRaComm(ILogger logger)
    {
        _logger = logger;
        _loraWan = new LoraWan(logger);
        _loraWan.PacketReceivedEvent += _loraWan_PacketReceivedEvent;
    }

    private void _loraWan_PacketReceivedEvent(object sender, PacketReceivedEventArgs e)
    {
        try
        {
            if (e.Packet.Type == PacketType.PULL_DATA)
            {
                UpdateEndPoint(e.Packet.GatewayId, e.EndPoint);
                _loraWan.Send(new Packet(PacketType.PULL_ACK, e.Packet.Token), e.EndPoint);
            }
            else if (e.Packet.Type == PacketType.PUSH_DATA)
            {
                _loraWan.Send(new Packet(PacketType.PUSH_ACK, e.Packet.Token), e.EndPoint);

                var rfPacket = JsonSerializer.Deserialize<RxPacket>(e.Packet.Json);

                if (rfPacket?.rxpk == null)
                {
                    _logger.Write(this, LogLevel.FINEST, e.Packet);
                }
                else
                {
                    foreach(var rxpk in rfPacket.rxpk)
                    {
                        _logger.Write(this, $"{e.Packet.GatewayId} | Received: {rxpk.data}");
                    }
                    
                    _logger.Write(this, LogLevel.DEBUG, e.Packet);
                }
            }
            else if (e.Packet.Type == PacketType.TX_ACK)
            {
                _logger.Write(this, LogLevel.INFO, e.Packet);
            }
        }
        catch (Exception ex)
        {
            _logger.Write(null, LogLevel.ERROR, ex);
        }
    }

    public void SendLoraWanPacket(string data)
    {
        var gatewayId = _gateways.Keys.FirstOrDefault();

        if (!_gateways.ContainsKey(gatewayId))
        {
            _logger.Write(this, LogLevel.ERROR, $"{gatewayId} | unknown gateway");
            return;
        }

        if (_gateways[gatewayId].EndPoint == null)
        {
            _logger.Write(this, LogLevel.ERROR, $"{gatewayId} | no endpoint");
            return;
        }

        byte[] dataBytes = System.Text.Encoding.UTF8.GetBytes(data);
        const int maxChunkSize = 255;
        int totalChunks = (int)Math.Ceiling((double)dataBytes.Length / maxChunkSize);

        for (int chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++)
        {
            try
            {
                int offset = chunkIndex * maxChunkSize;
                int currentChunkSize = Math.Min(maxChunkSize, dataBytes.Length - offset);

                byte[] chunkData = new byte[currentChunkSize];
                Array.Copy(dataBytes, offset, chunkData, 0, currentChunkSize);

                string base64Data = Convert.ToBase64String(chunkData);

                TxPk tx = new TxPk()
                {
                    imme = true,
                    freq = _gateways[gatewayId].TxFrequencies[0],
                    rfch = 0,
                    powe = _gateways[gatewayId].MaxPower,
                    modu = "LORA",
                    datr = "SF12BW125",
                    codr = "4/5",
                    size = (uint)chunkData.Length,
                    data = base64Data
                };

                Packet pullResp = new Packet(PacketType.PULL_RESP, Utils.RandomUshort())
                {
                    Json = JsonSerializer.Serialize(new TxPacket(tx)),
                    GatewayId = gatewayId
                };

                _logger.Write(this, LogLevel.FINEST, $"Sending chunk {chunkIndex + 1}/{totalChunks}");
                _loraWan.Send(pullResp, _gateways[gatewayId].EndPoint);                
            }
            catch (Exception ex)
            {
                _logger.Write(this, LogLevel.ERROR, $"Error sending chunk {chunkIndex + 1}: {ex.Message}");
            }
        }
    }



    private void UpdateEndPoint(string gatewayId, IPEndPoint endPoint)
    {
        _logger.Write(this, LogLevel.FINEST, $"{gatewayId} | check in");

        if (!_gateways.ContainsKey(gatewayId))
        {
            _gateways.Add(gatewayId, new Gateway());
        }

        if (!endPoint.Equals(_gateways[gatewayId].EndPoint))
        {
            _gateways[gatewayId].EndPoint = endPoint;
            _logger.Write(this, LogLevel.INFO, $"{gatewayId} | registered at {endPoint.Address}:{endPoint.Port}");
        }
    }

    public void AddGateway(string id, TxPowerMode power, params float[] txFrequencies)
    {
        _gateways.Add(id, new Gateway() { Id = id, PowerMode = power });
        _gateways[id].TxFrequencies.AddRange(txFrequencies);
    }

    public void AddBroadcast(string message)
    {
        _messages.Add(message);
    }

    public void Start()
    {
        _loraWan?.Start();
    }

    public void Stop()
    {
        _broadcastTimer?.Dispose(); // Stop the timer
        _loraWan?.Stop();
    }

    // New method: Start broadcasting random messages
    public void StartBroadcast(int intervalInMilliseconds)
    {
        //_broadcastTimer = new Timer(SendRandomMessage, null, 0, intervalInMilliseconds);
        _broadcastTimer = new Timer(SendNextMessage, null, 0, intervalInMilliseconds);
        _logger.Write(this, LogLevel.INFO, $"Random message broadcasting started with an interval of {intervalInMilliseconds} ms.");
    }

    private int _currentMessageIndex = 0;

    private void SendNextMessage(object? state)
    {
        if (_messages.Count == 0)
        {
            _logger.Write(this, LogLevel.WARNING, "No messages available to send.");
            return;
        }

        var currentMessage = _messages[_currentMessageIndex];
        _currentMessageIndex = (_currentMessageIndex + 1) % _messages.Count;

        _logger.Write(this, LogLevel.INFO, $"Broadcasting: {currentMessage}");
        SendLoraWanPacket(currentMessage);
    }

    private void SendRandomMessage(object? state)
    {
        if (_messages.Count == 0)
        {
            _logger.Write(this, LogLevel.WARNING, "No messages available to send.");
            return;
        }

        int index = _random.Next(0, _messages.Count);
        string randomMessage = _messages[index];

        _logger.Write(this, LogLevel.INFO, $"Broadcasting: {randomMessage}");
        SendLoraWanPacket(randomMessage);
    }
}
