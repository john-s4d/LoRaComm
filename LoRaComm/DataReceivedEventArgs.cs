using System.Net;


public class DataReceivedEventArgs
{
    public byte[] Data { get; set; }
    public IPEndPoint EndPoint { get; set; }
    public DataReceivedEventArgs(byte[] data, IPEndPoint endPoint)
    {
        Data = data;
        EndPoint = endPoint;
    }
}
