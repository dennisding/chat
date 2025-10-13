
global using ActorId = Common.ActorId;

using Server;
using Services;
using Protocol;

namespace ChatServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World server!");

            ActorServices services = new ActorServices();
            var server = new Server<IBasicClient, IBasicServer>(services);

            server.ServeForeverAt(999);
        }
    }
}
