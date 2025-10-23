
using Common;
using Services;
using System.Net.Sockets;

namespace Server;

public class ActorServices : IServices
{
    ActorId serviceId = Game.GenActorId();
    public ActorServices()
    {
    }

    public IConnection NewConnection(TcpClient client)
    {
        return new ActorConnection(client);
    }

    public void OnConnected(IConnection con)
    {
    }

    public void OnDisconnected(IConnection con)
    {
    }

    public void OnReady()
    {
        string actorType = Game.config.startActor;
        ActorId aid = Game.CreateActor(actorType);
        Game.SetCoreId(aid);
    }
}

public class ActorConnection: IConnection, IBasicServer
{
    public TcpClient client;
    public IBasicClient remote;
    ActorId aid;
    public IDispatcher<IBasicServer> dispatcher;
    public ActorConnection(TcpClient client)
    {
        this.client = client;
        ISender sender = new NetworkStreamSender(client.GetStream());
        this.remote = Common.Sender.Sender.Create<IBasicClient>(sender);

        this.dispatcher = Common.Dispatcher.Dispatcher.Create<IBasicServer>();
    }

    public void OnConnected()
    {
        string connectActor = Game.config.connectActor;
        Game.CreateActor(connectActor, OnConnectActorCreated);
    }

    public void OnConnectActorCreated(Actor actor)
    {
        Console.WriteLine($"OnLoginActorCreated: {actor.aid}, {actor}");
        // bind the actor client

        this.aid = actor.aid;

        this.remote.CreateActor(actor.typeName, aid);

        // bind the remote client
        this.remote.BindClientTo(aid);
        // bind the actor client
        actor.BindClient(this);
    }

    public void OnDisconnected()
    {
        if (aid.value != 0)
        {
            Game.DelActor(aid);
        }
    }

    public void DispatchMessage(BinaryReader data)
    {
        dispatcher.Dispatch(this, data);
    }

    // implement the protocol between client and server
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