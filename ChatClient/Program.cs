using Common;
using Services;
using System.Net.Sockets;

namespace ChatClient
{
    class ClientServices : Services.IClientServices<IServerMethod>, IClientMethod
    {
        IServerMethod? remote;
        TcpClient? client;

        InputMgr inputMgr;

        public ClientServices()
        {
            inputMgr = new InputMgr();
        }

        public void Tick()
        {
            inputMgr.Tick();
        }

        public void OnConnected(TcpClient _client, IServerMethod _remote)
        {
            remote = _remote;
            client = _client;

            Console.WriteLine("OnConnected");
            string msg = "msg from client!!";
            remote!.Echo(msg);
        }

        public void OnDisconnected()
        {
            Console.WriteLine("OnDisconnected");
        }

        public void EchoBack(string msg)
        {
            Console.WriteLine($"EchoBack: {msg}");
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World client!");

            ClientServices services = new ClientServices();
            var client = new Client<IClientMethod, IServerMethod>(services);

            client.Connect("127.0.0.1", 999);

            while (true)
            {
                client.Poll();
                services.Tick();

                Thread.Sleep(10);
            }
        }
    }
}
