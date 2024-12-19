using System.Net.Sockets;
using System.Net;

public class UdpClient
{
    public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs e);
    public event DataReceivedEventHandler? DataReceivedEvent;
    public bool IsStarted { get; private set; }

    private int _port;
    private Thread? _listenThread;
    private System.Net.Sockets.UdpClient? _listenClient;
    private ILogger _logger;

    public UdpClient(int port, ILogger logger)
    {
        _port = port;
        _logger = logger;
    }

    private void OnDataEvent(byte[] data, IPEndPoint endPoint)
    {
        DataReceivedEvent?.Invoke(this, new DataReceivedEventArgs(data, endPoint));
    }

    public void Start()
    {
        if (IsStarted) { return; }

        _logger.Write(this, LogLevel.INFO, "starting listener");

        IsStarted = true;

        _listenThread = new Thread(new ThreadStart(Receive));

        _listenThread.Start();
    }

    private void Receive()
    {
        _listenClient = new System.Net.Sockets.UdpClient(_port);

        _logger.Write(this, LogLevel.INFO, "listener thread started");

        while (IsStarted)
        {
            try
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                OnDataEvent(_listenClient.Receive(ref remoteEndPoint), remoteEndPoint);
            }
            catch (SocketException ex)
            {
                _logger.Write(this, LogLevel.ERROR, ex.ErrorCode == 10060 ? "timeout error" : $"serious error: {ex.ErrorCode}");
            }
        }
    }

    public async void Send(byte[] data, IPEndPoint endPoint)
    {
        try
        {
            await _listenClient.SendAsync(data, data.Length, endPoint);
        }
        catch (Exception e)
        {
            _logger.Write(this, LogLevel.ERROR, $"Send error: {e}");
        }
    }

    public void Stop()
    {
        if (!IsStarted) { return; }

        _logger.Write(this, LogLevel.INFO, "stopping listener");

        IsStarted = false;
        _listenClient.Close();
        _listenThread.Join(5000);
        _listenThread = null;

        _logger.Write(this, LogLevel.INFO, "listener thread stopped");
    }
}
