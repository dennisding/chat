

using System.Net;
using System.Net.Sockets;
using System.Threading.Channels;

namespace Services;

struct BroadcastMessage
{
    public byte[] data;
    public IPEndPoint endPoint;

    public BroadcastMessage(byte[] data, IPEndPoint endPoint)
    {
        this.data = data;
        this.endPoint = endPoint;
    }
}

public class Broadcast
{
    IBroadcastService services;
    UdpClient client;

    Channel<BroadcastMessage> channel;
    bool running = false;

    public Broadcast(IBroadcastService services)
    {
        this.services = services;
        this.client = new UdpClient();
        this.channel = Channel.CreateUnbounded<BroadcastMessage>();

        this.client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
    }

    public void JointMulticastGroup(string address, int port)
    {
        IPAddress ipAddress = IPAddress.Parse(address);
        client.Client.Bind(new IPEndPoint(IPAddress.Any, port));

        client.JoinMulticastGroup(ipAddress);

        running = true;

        Task.Run(Listen);
    }

    public void Poll()
    {

    }

    public async Task Listen()
    {
        while (running)
        {
            try
            {
                var result = await client.ReceiveAsync();
                byte[] data = result.Buffer;
                IPEndPoint endpoint = result.RemoteEndPoint;
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error Message: {e.Message}");
            }
        }
    }
}