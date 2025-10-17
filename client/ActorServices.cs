
using Common;
using Services;
using System.Net.Sockets;

namespace Client;

public class ActorServices : IClientServices, IBasicClient
{
    public TcpClient? client;
    public IBasicServer? remote;

    ActorId? currentActor;
    IDispatcher<IBasicClient> dispatcher;
    public ActorServices()
    {
        dispatcher = Common.Dispatcher.Dispatcher.Create<IBasicClient>();
        Game.Init();
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

    public void OnConnected(TcpClient client)
    {
        this.client = client;

        ISender sender = new NetworkStreamSender(client.GetStream());
        this.remote = Common.Sender.Sender.Create<IBasicServer>(sender);
    }

    public void OnDisconnected()
    {
    }

    public void DispatchMessage(BinaryReader reader)
    {
        dispatcher.Dispatch(this, reader);
    }

    public void AddActorType(string name, Type type)
    {
        Game.RegisterActor(name, type);
    }

    // Protcol implement
    public void Echo(string msg)
    {
        remote!.EchoBack(msg);
    }

    public void EchoBack(string msg)
    {
        Console.WriteLine($"EchoBack: {msg}");
    }

    public void CreateActor(string name, ActorId aid)
    {
        Console.WriteLine($"CreateActor:{name}, {aid}");
        Game.CreateActor(name, aid);
    }

    public void ActorMessage(ActorId aid, MemoryStream stream)
    {
        Console.WriteLine($"ActorMessage: {aid}, {stream.Length}");
        Actor actor = Game.GetActor(aid)!;
        BinaryReader reader = new BinaryReader(stream);
        actor.DispatchMessage(reader);
    }
}