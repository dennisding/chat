
using Common;

namespace Server;

public class Actor
{
    public ActorId aid;
    public string typeName = "";
    public ActorConnection? clientInfo;

    public Actor()
    {
    }

    public virtual void Init()
    {

    }

    public virtual void Finit()
    {
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
    }

    public virtual void GiveClientTo(Actor actor)
    {
    }

    public virtual void AttributeChanged(AttributeFlag flag, int index, MemoryStream data)
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
        // 清除Actors和aoi内的信息, 需要进一步考虑
        if (connection == null)
        {
            return;
        }

        // del self and other entity in client!
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

    public override void AttributeChanged(AttributeFlag flag, int index, MemoryStream data)
    {
//        base.AttributeChanged(flag, index, data);
        if ((flag & AttributeFlag.Client) != 0)
        {
        }
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