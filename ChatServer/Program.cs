
global using ActorId = Common.ActorId;

using System.Net.Sockets;

using Services;
using Common;

namespace server
{
    class ChatServices : IServices<IClientMethod>
    {
        public ChatServices() 
        {
            Game.Init();
        }

        public IConnection NewConnection(TcpClient client, IClientMethod remote)
        {
            ChatConnection connection = new ChatConnection(client, remote);
            return connection;
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

    public class ChatConnection: IConnection, IServerMethod
    {
        IClientMethod remote;
        TcpClient client;
        ActorId aid = new ActorId(0);

        public ChatConnection(TcpClient _client, IClientMethod _remote) 
        {
            client = _client;
            remote = _remote;
        }

        public void Disconnect()
        {
            client.Close();
        }

        public void OnConnected()
        {
            aid = Game.CreateActor("Login");

            Console.WriteLine("OnConnectionConnected");
        }

        public void OnDisconnected()
        {
            Game.DelActor(aid);

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
