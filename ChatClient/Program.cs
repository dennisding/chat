using Client;
using Common;
using Protocol;
using Services;
using System.Net.Sockets;

namespace ChatClient
{
    public interface Hello
    {
        void SayHello(string msg, ActorId aid);
        void Echo(string msg);
        void EchoBack(string msg);
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World client!");

            Init();
            
            ActorServices services = CreateActorServices();
            // 

            var client = new Client<IBasicClient, IBasicServer>(services);

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
}
