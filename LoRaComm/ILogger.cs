using System;
using System.Text;


    public enum LogLevel : int
    {
        NONE = 0,
        ERROR = 1,
        WARNING = 2,
        INFO = 3,
        DEBUG = 4,
        FINEST = 5
    }

    public interface ILogger
    {
        void Write(object sender, LogLevel level, string message);
        void Write(object sender, LogLevel level, byte[] bytes, Encoding encoding);
        void Write(object sender, LogLevel level, Exception ex);
        void Write(object sender, string message);
        void Write(object sender, byte[] bytes, Encoding encoding);
        void Write(object sender, Exception ex);        
        void Write(object sender, LogLevel level, Packet packet);        
    }
