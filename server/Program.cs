using System.Net.Sockets;

using Services;
using Common;

namespace server
{
    class ChatServices : IServices<IClientMethod>
    {
        public ChatServices() 
        {
        }

        public IConnection NewConnection(TcpClient client, IClientMethod remote)
        {
            return new ChatConnection(client, remote);
        }

        public void OnConnected(IConnection connection)
        {
            Console.WriteLine("OnConnected");
        }

        public void OnDisconnected(IConnection connection)
        {
            Console.WriteLine("OnDisconnected");
        }
    }

    class ChatConnection : IConnection, IServerMethod
    {
        IClientMethod remote;
        TcpClient client;
        public ChatConnection(TcpClient _client, IClientMethod _remote) 
        {
            client = _client;
            remote = (IClientMethod)_remote;
        }

        public void OnConnected()
        {
            Console.WriteLine("OnConnectionConnected");
        }

        public void OnDisconnected()
        {
            Console.WriteLine("OnConnectionDisconnected");
        }

        public void Echo(string msg)
        {
            Console.WriteLine($"msg from client!{msg}");

            remote!.EchoBack(msg);
        }
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World server!");

            ChatServices services = new ChatServices();
            var server = new Server<IClientMethod, IServerMethod>(services);

            server.ServeForeverAt(999);
        }
    }
}
