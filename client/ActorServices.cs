
using Common;
using Protocol;
using Services;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Client
{
    public class ActorServices : IClientServices<IBasicServer>, IBasicClient
    {
        public TcpClient? client;
        public IBasicServer? remote;
        ActorId? currentActor;
        public ActorServices()
        {
            Game.Init();
        }

        public void OnConnected(TcpClient client, IBasicServer remote)
        {
            this.client = client;
            this.remote = remote;
        }

        public void OnDisconnected()
        {
        }

        public void AddActorType(string name, Type type)
        {
            Game.RegisterActor(name, type);
        }

        public void Echo(string msg)
        {
            remote!.EchoBack(msg);
        }

        public void EchoBack(string msg)
        {
            Console.WriteLine($"EchoBack: {msg}");
        }

        // CreateEntity(aid, string TypeName, DataSerice)
        public void CreateActor(string name, ActorId aid)
        {
            Console.WriteLine($"CreateActor:{name}, {aid}");
            Game.CreateActor(name, aid);
        }

        public void BindClientTo(ActorId aid)
        {
            Console.WriteLine($"BindClientTo: {aid}");
            if (currentActor != null)
            {
                Actor lastActor = Game.GetActor((ActorId)currentActor)!;
                // unbind the client
                lastActor.BindClient(null);
            }

            currentActor = aid;
            Actor actor = Game.GetActor(aid)!;
            actor.BindClient(this);
        }

        public void ActorMessage(ActorId aid, MemoryStream stream)
        {
            Console.WriteLine($"OnActorMessage: {aid}");

            Actor actor = Game.GetActor(aid)!;
            BinaryReader reader = new BinaryReader(stream);
            actor.DispatchMessage(reader);
        }
    }
}