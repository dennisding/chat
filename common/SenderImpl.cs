
using System.Net.Sockets;

namespace Common;

public class NetworkStreamSender : ISender
{
    public NetworkStream stream;

    public NetworkStreamSender(NetworkStream stream)
    {
        this.stream = stream;
    }

    public void Send(MemoryStream data)
    {
        byte[] lenData = BitConverter.GetBytes((int)data.Length);

        stream.Write(lenData, 0, lenData.Length);
        stream.Write(data.GetBuffer(), 0, (int)data.Length);
        stream.Flush();
    }

    public void Close()
    {
        stream.Close();
    }
}

public class NullSender : ISender
{
    public void Send(MemoryStream data)
    {
    }

    public void Close()
    {
    }
}

public class VisitorSender : ISender
{
    public MemoryStream? data;

    public void Send(MemoryStream data)
    {
        this.data = data;
    }

    public void Close()
    {
    }
}
