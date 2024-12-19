
public class LogEventArgs
{
    public string Message { get; set; }
    public LogLevel Level { get; set; }
    public LogEventArgs(LogLevel level, string message)
    {
        Level = level;
        Message = message;
    }
}
