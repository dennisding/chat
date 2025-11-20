
using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;

namespace Services;

class ClientMessage
{
    public NetState state = NetState.None;
    public byte[]? data = null;

    public static ClientMessage Connect()
    {
        ClientMessage msg = new ClientMessage();
        msg.state = NetState.Connected;
        return msg;
    }

    public static ClientMessage Disconnect()
    {
        ClientMessage msg = new ClientMessage { state = NetState.Disconnected };
        return msg;
    }

    public static ClientMessage DataReceived(byte[] data)
    {
        ClientMessage msg = new ClientMessage { data = data };

        return msg;
    }
}

public class Client
{
    IClientServices services;

    TcpClient client;
    bool connected = false;
    Channel<ClientMessage> channel;

    public Client(IClientServices service)
    {
        this.services = service;
        this.client = new TcpClient();
        this.channel = Channel.CreateUnbounded<ClientMessage>();
    }

    public void Connect(string host, int port)
    {

        IPAddress addr = IPAddress.Parse(host);

        Task.Run(() => HandleReadAsync(host, port));
    }

    public void Poll()
    {
        if (!connected)
        {
            return;
        }
        // process the read and write
        try
        {
            while (channel.Reader.TryRead(out ClientMessage? msg))
            {
                try
                {
                    DispatchMessage(msg);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Dispatch Exception: {e}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Poll Exception: {ex}");
        }
    }

    void DispatchMessage(ClientMessage msg)
    {
        if (msg.data != null)
        {
            OnDataReceived(msg.data);
        }
        else if (msg.state == NetState.Connected)
        {
            OnConnected();
        }
        else if (msg.state == NetState.Disconnected)
        {
            OnDisconnected();
        }
    }

    void OnConnected()
    {
        services.OnConnected(client);
    }

    void OnDisconnected() 
    {
        connected = false;
        services.OnDisconnected();
    }

    void OnDataReceived(byte[] data)
    {
        if (data.Length == 0)
        {
            connected = false;
            return;
        }
        //MemoryStream stream = new MemoryStream(data);
        //BinaryReader reader = new BinaryReader(stream);

        services.DispatchMessage(data);
    }

    async Task HandleReadAsync(string host, int port)
    {
        try
        {
            IPAddress addr = IPAddress.Parse(host);
            await client.ConnectAsync(addr, port);
            connected = true;

            await channel.Writer.WriteAsync(ClientMessage.Connect());

            // read forever untill exception
            byte[] lenBuffer = new byte[sizeof(int)];
            NetworkStream stream = client.GetStream();
            while (true)
            {
                await stream.ReadExactlyAsync(lenBuffer);

                int len = BitConverter.ToInt32(lenBuffer);
                byte[] data = new byte[len];
                await stream.ReadExactlyAsync(data);

                await channel.Writer.WriteAsync(ClientMessage.DataReceived(data));
            }
        }
        catch (Exception) 
        {
            await channel.Writer.WriteAsync(ClientMessage.Disconnect());
        }
    }
}
