
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Channels;

namespace server
{
    enum NetState
    {
        None,
        Connected,
        Disconnected,
    }

    class Message
    {
        public NetState state = NetState.None;
        public int connectId = 0;
        public byte[]? data = null;
        public Connection? connection = null;

        public static Message Connect(int _connectId)
        {
            Message msg = new Message();
            msg.connectId = _connectId;
            return msg;
        }

        public static Message Data(Connection connection, byte[] data)
        {
            Message msg = new Message();
            msg.data = data;
            msg.connection = connection;

            return msg;
        }

        public static Message Disconnect(int connectId)
        {
            Message msg = new Message();
            msg.connectId = connectId;

            return msg;
        }
    }

    class Connection
    {
        public int connectId;
        public TcpClient client;
        public NetworkStream stream;

        public Connection(int _connectId, TcpClient _client)
        {
            connectId = _connectId;
            client = _client;
            stream = _client.GetStream();
        }
    }

    class Server
    {
        TcpListener? listener;
        bool running = false;
        Channel<Message> channel;
        Dictionary<int, Connection> connections;

        public Server() 
        {
            channel = Channel.CreateBounded<Message>(10);
            connections = new Dictionary<int, Connection>();
        }

        public void ServeAt(int port)
        {
            Console.WriteLine($"serve_at: {port}");
        }

        public void ServeForeverAt(int port)
        {
            Console.WriteLine($"server_at port: {port}");

            running = true;
            
            IPAddress addr = IPAddress.Parse("127.0.0.1");
            listener = new TcpListener(addr, port);

            listener.Start();

            try
            {
                Task listen = Task.Run(Listen);
                // working thread
                Task process = Task.Run(Process);

                Task.WaitAll(listen, process);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"serverr error:{ex.Message}");
            }
            finally
            {
                Stop();
            }
        }

        async Task Process()
        {
            while (running)
            {
                await foreach(var message in channel.Reader.ReadAllAsync())
                {
                    // no blocking message
                    ProcessMessage(message);
                }
            }
        }

        void ProcessMessage(Message msg)
        {
            if (msg.data != null)
            {
                // process the data packet
                OnDataReceived(msg);
            }
            // dispatch the message
            if (msg.state == NetState.Connected)
            {
                Console.WriteLine($"connected!{msg.connectId}");
            }
            else if (msg.state == NetState.Disconnected)
            {
                Console.WriteLine($"disconnected!{msg.connectId}");
            }
        }

        void OnDataReceived(Message message)
        {
            Connection connection = message.connection!;
            
            string msg = Encoding.Unicode.GetString(message.data!);

            Console.WriteLine($"on data received: {msg.Length}, {msg}");

            // write back to client
            // write length
            ReadOnlySpan<byte> msgSpan = MemoryMarshal.AsBytes(msg.AsSpan());
            byte[] lenByte = BitConverter.GetBytes(msgSpan.Length);

            connection.stream.Write(lenByte);
            connection.stream.Write(msgSpan);
//            ReadonlySp MemoryMarshal.AsBytes(msg.AsSpan());

        }

        // listening task
        async Task Listen()
        {
            int connectId = 100;

            while (running)
            {
                if (listener == null)
                {
                    running = false;
                    break;
                }

                TcpClient client = await listener.AcceptTcpClientAsync();

                Console.WriteLine($"client connected: {client.Client.RemoteEndPoint}");

                Connection connection = new Connection(connectId, client);

                connections.Add(connectId, connection);
                Task _ = Task.Run(() => HandleConnection(connection));

                connectId++;
            }
        }

        async Task HandleConnection(Connection connection)
        {
            Console.WriteLine($"handle connection: {connection.connectId}");
            NetworkStream stream = connection.stream;
            while (running)
            {
                try
                {
                    byte[] lenBuff = new byte[sizeof(int)];
                    await stream.ReadExactlyAsync(lenBuff);

                    int len = BitConverter.ToInt32(lenBuff, 0);

                    Console.WriteLine($"read length: {len}");

                    byte[] dataBuff = new byte[len];
                    await stream.ReadExactlyAsync(dataBuff);
                    // data readed
                    //MemoryStream memory = new MemoryStream(dataBuff);
                    //BinaryReader reader = new BinaryReader(memory);
                    // send to process task

                    await channel.Writer.WriteAsync(Message.Data(connection, dataBuff));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"exception in reading: {ex.Message}");
                    await channel.Writer.WriteAsync(Message.Disconnect(connection.connectId));
                    break;
                }
                finally
                {
                    // Console.WriteLine("disconnected!");
                    // connection.channel.Send(Message::Connected());
                }
            }
        }

        void Stop()
        {
            running = false;
            listener?.Stop();
            Console.WriteLine("server stop");
        }
    }
}