
using Common;
using Services;
using System.Net.Sockets;

namespace Client;

public class ActorServices : IClientServices, IBasicClient
{
    public TcpClient? client;
    public IBasicServer? remote;

    ActorId currentActor = new ActorId();
    IDispatcher dispatcher;

    public ActorServices()
    {
        dispatcher = Common.ProtocolCreator.CreateDispatcher<IBasicClient>();
        Game.Init();
    }

    public void BindClientTo(ActorId aid)
    {
        Actor? oldActor = Game.GetActor(currentActor!);
        if (oldActor != null)
        {
            oldActor.BindClient(null);
        }

        currentActor = aid;
        Game.SetPlayer(aid);
        Actor actor = Game.GetActor(aid)!;
        actor.BindClient(this);
    }

    public void OnConnected(TcpClient client)
    {
        this.client = client;

        ISender sender = new NetworkStreamSender(client.GetStream());
        remote = new IBasicServer_Packer(sender);
    }

    public void OnDisconnected()
    {
    }

    public void DispatchMessage(byte[] data)
    {
        var stream = new MemoryStream(data);
        var reader = new MemoryStreamDataStreamReader(stream, PropertyFlag.Client);

        dispatcher.Dispatch(reader, this);
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

    public void CreateActor(string name, ActorId aid, MemoryStream properties)
    {
        BinaryReader reader = new BinaryReader(properties);
        Game.CreateActor(name, aid, reader);
    }

    public void DelActor(ActorId aid)
    {
        Console.WriteLine($"DelActor: {aid}");
        Game.DelActor(aid);
    }

    public void ActorMessage(ActorId aid, MemoryStream stream)
    {
        Actor actor = Game.GetActor(aid)!;
        actor.DispatchMessage(stream);
    }

    public void ActorPropertyChanged(ActorId aid, int index, MemoryStream stream)
    {
        Actor? actor = Game.GetActor(aid);
        actor?.OnPropertyChanged(index, stream);
    }
}