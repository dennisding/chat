using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Swift;
using System.Text;
using System.Threading.Channels;

namespace client
{
    //enum NetState
    //{
    //    None,
    //    Connect,
    //    Disconnect,
    //}

    //class Message
    //{
    //    public NetState State = NetState.None;

    //    public byte[]? data;

    //    public static Message Connect()
    //    {
    //        Message msg = new Message();
    //        msg.State = NetState.Connect;

    //        return msg;
    //    }

    //    public static Message Data(byte[] data)
    //    {
    //        Message msg = new Message();
    //        msg.data = data;

    //        return msg;
    //    }
    //}

    //class Client
    //{
    //    bool connected = false;
    //    TcpClient client;
    //    Channel<Message> channel;
    //    NetworkStream? stream = null;

    //    public Client()
    //    {
    //        client = new TcpClient();
    //        channel = Channel.CreateUnbounded<Message>();
    //    }

    //    public void Connect(string ip, int port)
    //    {
    //        IPAddress address = IPAddress.Parse(ip);
    //        client.Connect(address, port);

    //        Task.Run(() => Read());
    //        connected = true;

    //        Console.WriteLine("Connected!");
    //        stream = client.GetStream();

    //        OnConnected();
    //    }

    //    void OnConnected()
    //    {
    //        // echo client
    //        string msg = "msg from client!!!!";
    //        ReadOnlySpan<byte> rawBytes = MemoryMarshal.AsBytes(msg.AsSpan());

    //        byte[] len = BitConverter.GetBytes(rawBytes.Length);

    //        stream?.Write(len, 0, len.Length);
    //        stream?.Write(rawBytes);
    //    }

    //    public void Poll()
    //    {
    //        if (stream == null)
    //        {
    //            return;
    //        }

    //        Message? msg;
    //        while (channel.Reader.TryRead(out msg))
    //        {
    //            DispatchMessage(msg);
    //        }
    //    }

    //    void DispatchMessage(Message message)
    //    {
    //        if (message.data != null)
    //        {
    //            string msg = Encoding.Unicode.GetString(message.data);
    //            // packet received
    //            Console.WriteLine($"on message received! {msg.Length}, {msg}");
    //        }
    //    }

    //    public async Task Read()
    //    {
    //        // make the compiler happy
    //        while (connected)
    //        {
    //            byte[] lenBuff = new byte[sizeof(int)];
    //            await stream!.ReadExactlyAsync(lenBuff);

    //            int len = BitConverter.ToInt32(lenBuff, 0);
    //            byte[] data = new byte[len];

    //            await stream.ReadExactlyAsync(data);

    //            await channel.Writer.WriteAsync(Message.Data(data));
    //        }
    //    }
    //}

    class ClientServices : services.IConnection
    {
        TcpClient? client = null;

        public ClientServices()
        {
        }

        public void OnConnected(TcpClient client)
        {
            Console.WriteLine("OnConnected");
            this.client = client;

            // send the msg to client
            string msg = "msg from client";
            SendString(msg);
        }

        void SendString(string msg)
        {
            ReadOnlySpan<byte> buff = MemoryMarshal.AsBytes(msg.AsSpan());
            byte[] lenBuffer = BitConverter.GetBytes(buff.Length);

            NetworkStream stream = client!.GetStream();

            stream.Write(lenBuffer);
            stream.Write(buff);
            stream.Flush();

            Console.WriteLine($"send data to server: {buff.Length}");
        }

        public void OnDisconnected() 
        {
            Console.WriteLine("OnDisconnected");
        }

        public void DispatchRpc(byte[] data)
        {
            Console.WriteLine("dispatch rpc!!!");

            string msg = Encoding.Unicode.GetString(data);
            Console.WriteLine($"msg from server, {msg}");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            ClientServices services = new ClientServices();

            services.Client client = new services.Client(services);

            client.Connect("127.0.0.1", 999);

            while (true)
            {
                client.Poll();

                Thread.Sleep(10);
            }

            //Client client = new Client();

            //client.Connect("127.0.0.1", 999);

            //while (true)
            //{
            //    client.Poll();

            //    Thread.Sleep(1);
            //}

            //TcpClient client = new TcpClient();

            //IPAddress addr = IPAddress.Parse("127.0.0.1");

            //client.Connect(addr, 999)
        }
    }
}
