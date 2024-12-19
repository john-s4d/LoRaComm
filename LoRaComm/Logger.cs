using System.Text;

public class Logger : ILogger
{
    public delegate void LogEventHandler(object sender, LogEventArgs e);
    public event LogEventHandler LogEvent;

    private const LogLevel DEFAULT = LogLevel.INFO;

    private const long MAX_LENGTH = 0x100000L;
    private string _logFile;

    private static Mutex _mutex = new Mutex(false);

    public LogLevel FileLevel { get; set; } = LogLevel.DEBUG;

    public Logger(string logFile)
    {
        _logFile = logFile;
    }

    private async Task WriteToFile(LogLevel level, string line)
    {
        if (level > FileLevel) { return; }

        bool rollFile;

        _mutex.WaitOne();

        using (var file = new StreamWriter(_logFile, append: true))
        {
            await file.WriteLineAsync(line);

            rollFile = file.BaseStream.Length > MAX_LENGTH;
        }

        if (rollFile)
        {
            FileInfo fi = new FileInfo(_logFile);
            if (fi.Exists)
            {
                fi.MoveTo($"{_logFile}_{DateTime.Now.ToFileTimeUtc()}.txt");
            }
        }
        _mutex.ReleaseMutex();
    }
    private void OnLogEvent(LogLevel level, string message)
    {
        LogEvent?.Invoke(this, new LogEventArgs(level, message));
    }

    public void Write(object sender, LogLevel level, string message)
    {
        message = $"{DateTime.Now.ToString("MM-dd-yyyy HH:mm:ss.fff")} | {Thread.CurrentThread.ManagedThreadId} | {message}";
        WriteToFile(level, message).Wait();
        OnLogEvent(level, message);
    }

    public void Write(object sender, LogLevel level, byte[] bytes, Encoding encoding)
    {
        Write(sender, level, encoding.GetString(bytes));
    }

    public void Write(object sender, LogLevel level, Exception ex)
    {
        Write(sender, level, ex.Message);
        Write(sender, LogLevel.DEBUG, ex.StackTrace);
    }

    public void Write(object sender, string line)
    {
        Write(sender, DEFAULT, line);
    }

    public void Write(object sender, byte[] bytes, Encoding encoding)
    {
        Write(sender, DEFAULT, bytes, encoding);
    }

    public void Write(object sender, Exception ex)
    {
        Write(sender, DEFAULT, ex);
    }

    public void Write(object sender, LogLevel level, Packet packet)
    {
        Write(sender, level, $"{packet.GatewayId} | {packet.Type.ToString().PadRight(9, ' ')} | {packet.Token.ToString().PadLeft(5, ' ')} | {packet.Json}");
    }
}
