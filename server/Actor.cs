
using Common;

namespace Server;

public class Actor
{
    public ActorId aid;
    public string typeName = "";
    public World? world;
    public ActorConnection? clientInfo;

    public bool inWorld
    {
        get { return world != null; }
    }

    public Actor()
    {
    }

    public virtual void Init()
    {

    }

    public virtual void Finit()
    {
    }

    public virtual void EnterWorld(World _world)
    {
        world = _world;
    }

    public virtual void LeaveWorld()
    {
        world = null;
    }

    public virtual void BindClient(ActorConnection? client)
    {
        this.clientInfo = client;
    }

    public virtual void OnClientBinded()
    {
    }

    public virtual void OnClientUnbinded()
    {

    }

    public virtual void DispatchMessage(MemoryStream stream)
    {
    }

    public virtual void BecomePlayer()
    {
    }

    public virtual void DestroySelf()
    {
        Game.DelActor(aid);
    }

    public virtual void GiveClientTo(Actor actor)
    {

    }
}

public class ActorServer<ClientImpl, CoreImpl> : Actor
    where ClientImpl: class
    where CoreImpl: class
{
    public ClientImpl? client;

    ActorConnection? connection;
    IDispatcher<CoreImpl> dispatcher = Protocol.Dispatcher.Dispatcher.Create<CoreImpl>();

    public ActorServer()
    {
        client = null;
    }

    public override void BindClient(ActorConnection? con)
    {
        base.BindClient(con);
        // reset the clint and shadow
        if (con == null)
        {
            client = null;
            connection = null;
            OnClientUnbinded();
            return;
        }

        ISender sender = new ClientSender(aid, con);
        client = Protocol.Sender.Sender.Create<ClientImpl>(sender);
        connection = con;

        OnClientBinded();
    }

    public override void DispatchMessage(MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);
        dispatcher.Dispatch((this as CoreImpl)!, reader);
    }

    public override void GiveClientTo(Actor actor)
    {
        base.GiveClientTo(actor);
        ActorConnection connection = this.connection!;
        BindClient(null);

        actor.BindClient(connection);
    }
}

public class ActorShadow<IClient, ICore>: Actor
{
    public ActorShadow()
    {

    }
}

class ClientSender : ISender
{
    ActorConnection connection;
    ActorId aid;
    public ClientSender(ActorId aid, ActorConnection con)
    {
        this.aid = aid;
        this.connection = con;
    }

    public void Send(MemoryStream data)
    {
        connection.remote.ActorMessage(aid, data);
    }

    public void Close()
    {
        connection.client.Close();
    }
}