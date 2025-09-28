
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace services
{
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
        IConnection connection;
        TcpClient client;
        bool connected = false;
        Channel<ClientMessage> channel;

        public Client(IConnection connection)
        {
            this.connection = connection;
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
                while (true)
                {
                    if (channel.Reader.TryRead(out ClientMessage? msg))
                    {
                        try
                        {
                            DispatchMessage(msg);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Dispatch exception: {e}");
                        }
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
            connection.OnConnected(client);
        }

        void OnDisconnected() 
        {
            Console.WriteLine("OnDisconnected");
            connected = false;
            connection.OnDisconnected();
        }

        void OnDataReceived(byte[] data)
        {
            Console.WriteLine($"OnDataReceived: {data.Length}");
            connection.DispatchRpc(data);
        }

        async Task HandleReadAsync(string host, int port)
        {
            Console.WriteLine("handle async read!!!");
            try
            {
                // connect
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
}
