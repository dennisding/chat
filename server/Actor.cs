
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

public class ActorServer<ClientImpl, ServerImpl> : Actor
    where ClientImpl: class
    where ServerImpl: class
{
    public ClientImpl? client;

    ActorConnection? connection;
    IDispatcher<ServerImpl> dispatcher = Protocol.Dispatcher.Dispatcher.Create<ServerImpl>();

    public ActorServer()
    {
        client = null;
    }

    public override void BindClient(ActorConnection? con)
    {
        // reset the clint and shadow
        if (con == null)
        {
            ClearActors();
            base.BindClient(null);

            client = null;
            connection = null;
            OnClientUnbinded();
            return;
        }

        // 绑定客户端之前的准备工作
        con.remote.CreateActor(this.typeName, this.aid);
        con.remote.BindClientTo(this.aid);

        base.BindClient(con);

        ISender sender = new ClientSender(aid, con);
        client = Protocol.Sender.Sender.Create<ClientImpl>(sender);
        connection = con;

        OnClientBinded();
    }

    public void ClearActors()
    {
        // 清除Actors和aoi内的信息, 需要进一步分析考虑
        if (connection == null)
        {
            return;
        }

        connection.remote.DelActor(this.aid);
    }

    public override void DispatchMessage(MemoryStream stream)
    {
        BinaryReader reader = new BinaryReader(stream);
        dispatcher.Dispatch((this as ServerImpl)!, reader);
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