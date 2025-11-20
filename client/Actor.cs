using Common;
using Protocol;
using System.Reflection;

namespace Client;

public class Actor
{
    public ActorId aid;
    ActorServices? services;

    public virtual void Init()
    {

    }

    public virtual void Finit()
    {

    }

    public virtual void EnterWorld()
    {

    }

    public virtual void LeaveWorld()
    {

    }

    public virtual void BindClient(ActorServices? services)
    {
        this.services = services;
    }

    public virtual void OnClientBinded()
    {
    }

    public virtual void DispatchMessage(MemoryStream stream)
    {

    }

    public virtual void OnPropertyChanged(int index, MemoryStream stream)
    {
    }

    public virtual void UnpackProperty(BinaryReader? reader)
    {
    }
}

public class ActorClient<ClientImpl, ServerImpl, DataImpl> : Actor, IPropertyOwner
    where ServerImpl : class
    where DataImpl : Property, IProperty, new()
{
    public ServerImpl? server;
    public DataImpl props;
    public IDispatcher dispatcher;

    public ActorClient()
    {
        props = new DataImpl();
        dispatcher = Protocol.ProtocolCreator.CreateDispatcher<ClientImpl>();
    }

    public override void BindClient(ActorServices? services) 
    {
        base.BindClient(services);

        if (services == null)
        {
            server = null;
            return;
        }

        ISender sender = new ActorSender(aid, services);
        server = Protocol.ProtocolCreator.CreatePacker<ServerImpl>(sender, PropertyFlag.Client);

        OnClientBinded();
    }

    public override void DispatchMessage(MemoryStream stream)
    {
        var reader = new MemoryStreamDataStreamReader(stream);
        dispatcher.Dispatch(reader, this);
    }

    public override void OnPropertyChanged(int index, MemoryStream stream)
    {
        var reader = new MemoryStreamDataStreamReader(stream, PropertyFlag.Client);
        this.props.UnpackProperty(index, reader);
    }

    public void OnPropertyChanged(PropertyInfomation info)
    {
        MethodInfo? notifier = this.GetType().GetMethod(info.notifierName);
        notifier?.Invoke(this, null);
    }

    public override void UnpackProperty(BinaryReader? reader)
    {
        if (reader == null)
        {
            return;
        }

        this.props.SetNotify(false);

//        this.props.UnpackFrom(reader);

        this.props.SetNotify(true);
    }
}

class ActorSender : ISender
{
    ActorServices services;
    ActorId aid;
    public ActorSender(ActorId aid, ActorServices services)
    {
        this.aid = aid;
        this.services = services;
    }

    public void Send(MemoryStream stream)
    {
        services.remote!.ActorMessage(aid, stream);
    }

    public void Close()
    {
        services.client!.Close();
    }
}