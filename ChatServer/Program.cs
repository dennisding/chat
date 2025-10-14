
global using ActorId = Common.ActorId;

using Server;
using Services;
using Common;

namespace ChatServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World server!");

            Init();

            ActorServices services = new ActorServices();
            var server = new Server<IBasicClient, IBasicServer>(services);

            server.ServeForeverAt(999);
        }

        static void Init()
        {
            Common.Initer.Init();
            Server.Initer.Init();

            Game.RegisterActor("Login", typeof(LoginCore));
        }
    }
}
