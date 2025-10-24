
global using ActorId = Common.ActorId;

using Server;
using Services;
using Common;

namespace ChatServer;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World server!");

        Init();

        ActorServices services = new ActorServices();
        var server = new Services.Server(services);

        server.ServeForeverAt(999);
    }

    static void Init()
    {
        Common.Initer.Init();
        Server.Initer.Init();

        Config config = new Config();

        Game.Init(config);

        RegisterActors();
    }

    static void RegisterActors()
    {
        // 服务器启动时会创建一个服务器Actor
        Game.RegisterActor("Server", typeof(ServerActor));
        // 建立连接是会有一个LoginActor
        Game.RegisterActor("Login", typeof(LoginServer));
        Game.RegisterActor("Chat", typeof(ChatServer));
        Game.RegisterActor("Room", typeof(RoomServer));
    }
}
