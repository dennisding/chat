
using Common;
using Services;
using System.Net.Sockets;

namespace Server
{
    public class ActorServices : IServices<IBasicClient>, IBasicServer
    {
        ActorId serviceId = Game.GenActorId();
        public ActorServices()
        {
        }

        public IConnection NewConnection(TcpClient client, IBasicClient remote)
        {
            return new ActorConnection(client, remote);
        }

        public void OnConnected(IConnection con)
        {
        }

        public void OnDisconnected(IConnection con)
        {
        }

        public void Echo(string msg)
        {
            Console.WriteLine($"ActorServices.Echo: {msg}");
        }

        public void EchoBack(string msg)
        {
            Console.WriteLine($"ActorServices.EchoBack: {msg}");
        }

        public void ActorMessage(ActorId aid, MemoryStream stream)
        {
            Actor actor = Game.GetActor(aid)!;
            Console.WriteLine($"ActorServices.ActorMessage: {aid}");
        }
    }

    public class ActorConnection: IConnection, IBasicServer
    {
        public TcpClient client;
        public IBasicClient remote;
        ActorId aid;
        public ActorConnection(TcpClient client, IBasicClient remote)
        {
            this.client = client;
            this.remote = remote;
        }

        public void OnConnected()
        {
            aid = Game.CreateActor("Login", OnActorCreated);
        }

        public void OnActorCreated(string name, ActorId aid, Actor actor)
        {
            Console.WriteLine($"OnActorCreated: {aid}, {actor}");
            // bind the actor client

            this.aid = aid;

            // 这里的流程需要进一步思考
            this.remote.CreateActor(name, aid);

            // bind the remote client
            this.remote.BindClientTo(aid);
            // bind the actor client
            actor.BindClient(this);
            // actor.client.becomePlayer()
        }

        public void OnDisconnected()
        {
            if (aid.value != 0)
            {
                Game.DelActor(aid);
            }
        }

        public void Echo(string msg)
        {
            Console.WriteLine($"Connection.Echo: {msg}");
        }

        public void EchoBack(string msg)
        {
            Console.WriteLine($"Connection.EchoBack: {msg}");
        }

        public void ActorMessage(ActorId aid, MemoryStream msg)
        {
            Console.WriteLine($"Connection.ActorMessage: {aid}, {msg.Length}");
            Actor? actor = Game.GetActor(aid);

            actor!.DispatchMessage(msg);
        }
    }
}