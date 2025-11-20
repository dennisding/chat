
using Common;
using System.Reflection;

namespace Server;

public class Actor
{
    public ActorId aid;
    public string typeName = "";
    public ActorConnection? clientInfo;

    bool propertyNotify = false;

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

    public virtual void OnReceiveMail(MemoryStream stream)
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

    public void SetPropertyNotify(bool notify)
    {
        this.propertyNotify = notify;
    }
}

public class ActorServer<ClientImpl, ServerImpl, DataImpl> : Actor, IPropertyOwner
    where ClientImpl: class
    where ServerImpl: class
    where DataImpl: Common.Property, new()
{
    public ClientImpl? client;
    public DataImpl props = new DataImpl();

    ActorConnection? connection;
    IDispatcher dispatcher;

    public ActorServer()
    {
        client = null;
        props.SetOwner(this);
        dispatcher = Protocol.ProtocolCreator.CreateDispatcher<ServerImpl>();
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
        MemoryStream stream = new MemoryStream();

        con.remote.CreateActor(this.typeName, this.aid, stream);
        con.remote.BindClientTo(this.aid);

        base.BindClient(con);

        ISender sender = new ClientSender(aid, con);
        client = Protocol.ProtocolCreator.CreatePacker<ClientImpl>(sender, PropertyFlag.Client);
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
        var reader = new MemoryStreamDataStreamReader(stream, PropertyFlag.Client);
        dispatcher.Dispatch(reader, this);
    }

    public override void OnReceiveMail(MemoryStream stream)
    {
        DispatchMessage(stream);
    }

    public override void GiveClientTo(Actor actor)
    {
        base.GiveClientTo(actor);
        ActorConnection connection = this.connection!;
        BindClient(null);

        actor.BindClient(connection);
    }

    public void OnPropertyChanged(Common.PropertyInfomation info)
    {
        MethodInfo? notifier = this.GetType().GetMethod(info.notifierName);
        notifier?.Invoke(this, null);
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