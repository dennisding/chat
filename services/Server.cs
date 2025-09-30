
using System.Net.Sockets;
using System.Threading.Channels;

namespace Services
{
    public class Server<IClient, IServer>
    {
        bool running = false;
        Dictionary<int, ConnectionInfo> connections;
        TcpListener? listener = null;
        IServices<IClient> services;
        Dispatcher dispatcher;
        Channel<Message> channel;

        public Server(IServices<IClient> services)
        {
            this.services = services;
            dispatcher = DispatcherBuilder.Build(typeof(IServer));

            connections = new Dictionary<int, ConnectionInfo>();
            channel = Channel.CreateUnbounded<Message>();
        }

        public void ServeForeverAt(int port)
        {
            Console.WriteLine($"ServeForeverAt: {port}");
            listener = TcpListener.Create(port);
            listener.Start();

            running = true;

            Task listen = Task.Run(Listen);
            Task process = Task.Run(Process);

            Task.WaitAll(listen, process);
        }

        async Task Listen()
        {
            int connectId = 100;

            while (running)
            {
                try
                {
                    TcpClient client = await listener!.AcceptTcpClientAsync();

                    Console.WriteLine($"Client Connected: {connectId}, {client.Client.RemoteEndPoint}");

//                    IConnection connection = services.NewConnection(client);
                    ConnectionInfo info = new ConnectionInfo(connectId, client);

                    await channel.Writer.WriteAsync(Message.Connect(info));

                    // crate new async function to read the data
                    Task _ = Task.Run(() => HandleClientRead(info));
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Server Error: {e}");
                    running = false;
                }

                connectId++;
            }
        }

        async void HandleClientRead(ConnectionInfo info)
        {
            byte[] lenBuffer = new byte[sizeof(int)];
            NetworkStream stream = info.stream;
            while (running)
            {
                try
                {
                    await stream.ReadExactlyAsync(lenBuffer);
                    int len = BitConverter.ToInt32(lenBuffer);

                    byte[] data = new byte[len];
                    await stream.ReadExactlyAsync(data);

                    await channel.Writer.WriteAsync(Message.DataReceived(info, data));
                }
                catch (Exception)
                {
                    await channel.Writer.WriteAsync(Message.Disconnect(info));
                    info.clinet.Close();
                    break;
                }
            }
        }

        async Task Process()
        {
            while (running)
            {
                try
                {
                    await foreach (var msg in channel.Reader.ReadAllAsync())
                    {
                        DispatchMessage(msg);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        void DispatchMessage(Message msg)
        {
            if (msg.data != null)
            {
                OnDataReceived(msg.info!, msg.data);
            }
            else if (msg.netState == NetState.Connected)
            {
                OnConnected(msg.info!);
            }
            else if (msg.netState == NetState.Disconnected)
            {
                OnDisconnected(msg.info!);
            }
        }

        void OnConnected(ConnectionInfo info)
        {
            connections.Add(info.connectId, info);

            IClient remote = RemoteBuilder.Build<IClient>(info.clinet);
            info.connection = services.NewConnection(info.clinet, remote);
            
            services.OnConnected(info.connection);
            info.connection.OnConnected();
        }

        void OnDisconnected(ConnectionInfo info)
        {
            info.connection!.OnDisconnected();
        }

        void OnDataReceived(ConnectionInfo info, byte[] data)
        {
            MemoryStream stream = new MemoryStream(data);
            BinaryReader reader = new BinaryReader(stream);

            dispatcher.Dispatch(info.connection!, reader);
        }
    }
}
