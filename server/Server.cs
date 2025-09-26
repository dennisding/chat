
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices.Swift;
using System.Security.Cryptography;
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

        public static Message Connect(int _connectId)
        {
            Message msg = new Message();
            msg.connectId = _connectId;
            return msg;
        }
    }

    class Connection
    {
        public int connectId;

        Connection(int _connectId)
        {
            connectId = _connectId;
        }
    }

    class Server
    {
        TcpListener? listener;
        bool running = false;
        Channel<Message> channel;

        public Server() 
        {
            channel = Channel.CreateBounded<Message>(10);
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

                while (running)
                {
                    Thread.Sleep(1);
                }
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

        async void Process()
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
            // dispatch the message
            if (msg.state == NetState.Connected)
            {
                Console.WriteLine($"connected!!!{msg.connectId}");
            }
            else if (msg.state == NetState.Disconnected)
            {
                Console.WriteLine($"disconnected!{msg.connectId}");
            }
        }

        // listening task
        async void Listen()
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

                Task _ = Task.Run(() => HandleConnection(connectId, client));

                connectId++;
            }
        }

        async Task HandleConnection(int connectId, TcpClient client)
        {
            Console.WriteLine($"handle connection: {connectId}");
        }

        void Stop()
        {
            running = false;
            listener?.Stop();
            Console.WriteLine("server stop");
        }
    }
}