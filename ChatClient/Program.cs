using Client;
using Common;
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

        while (true)
        {
            client.Poll();

            Thread.Sleep(10);
        }
    }

    static ActorServices CreateActorServices()
    {
        ActorServices services = new ActorServices();

        services.AddActorType("Login", typeof(LoginActor));

        return services;
    }

    static void Init()
    {
        Common.Initer.Init();
    }
}
