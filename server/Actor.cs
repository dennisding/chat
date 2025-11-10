
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

    public virtual void BecomePlayer()
    {
    }

    public virtual void DestroySelf()
    {
    }

    public virtual void GiveClientTo(Actor actor)
    {
    }

    public virtual void AttributeChanged(PropertyFlag flag, int index, MemoryStream data)
    {
    }

    public virtual void PropertyChanged(PropertyFlag flag, int index, 
        Action<MemoryStream> packer, 
        Action updator)
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
    IDispatcher<ServerImpl> dispatcher = Protocol.Dispatcher.Dispatcher.Create<ServerImpl>();

    public ActorServer()
    {
        client = null;
        props.SetOwner(this);
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

    public override void AttributeChanged(PropertyFlag flag, int index, MemoryStream data)
    {
        if ((flag & PropertyFlag.OwnerClient) != 0)
        {
            // send own client
        }
    }

    public override void PropertyChanged(PropertyFlag flag, int index, 
        Action<MemoryStream> packer, 
        Action notifier)
    {
        if ((flag & PropertyFlag.Client) != 0)
        {
            MemoryStream stream = new MemoryStream();
            packer(stream);
        }

        // notify changed!!!
        notifier();
    }

    public void OnPropertyChanged(Common.PropertyInfo info)
    {
        MethodInfo? notifier = this.GetType().GetMethod(info.notifierName);
        notifier?.Invoke(this, null);

        // notify to client!!!!
        MemoryStream stream = new MemoryStream();
        info.packer(this.props, stream);
        connection!.remote.ActorPropertyChanged(this.aid, stream);
//        connection!.remote.PropertyChanged(this.aid, stream);
        // send to client
        //MethodInfo? method = this.GetType().GetMethod(info.notifierName);
        //method?.Invoke(this, null);

        //MemoryStream stream = new MemoryStream();
        //info.packer(this.props, stream);
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