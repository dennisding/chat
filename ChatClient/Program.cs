using Client;
using Common;
using Protocol;
using Services;

namespace ChatClient;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World client!");

        Init();
        
        ActorServices services = CreateActorServices();
        var client = new Services.Client(services);

        client.Connect("127.0.0.1", 999);

        CommandMgr command = new CommandMgr();

        while (Game.running)
        {
            client.Poll();

            command.Tick();

            Thread.Sleep(10);
        }
    }

    static ActorServices CreateActorServices()
    {
        ActorServices services = new ActorServices();

        AddActors(services);

        return services;
    }

    static void AddActors(ActorServices services)
    {
        services.AddActorType("Login", typeof(LoginClient));
        services.AddActorType("Chat", typeof(ChatClient));
    }

    static void AddDataImpl()
    {
        // 后续可以通过代码生成器来自动注册生成
        Packer.RegisterType(typeof(ChatData), typeof(ClientChatData));
    }

    static void Init()
    {
        Common.Initer.Init();

        AddDataImpl();

        Game.Init();
    }
}
