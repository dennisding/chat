using Common;
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

    public virtual void DispatchMessage(BinaryReader reader)
    {

    }

    public virtual void OnPropertyChanged(BinaryReader reader)
    {

    }
}

public class ActorClient<ClientImpl, ServerImpl, DataImpl> : Actor, IPropertyOwner
    where ServerImpl: class
    where ClientImpl : class
    where DataImpl : Common.Property, new()
{
    public ServerImpl? server;
    IDispatcher<ClientImpl> dispatcher = Protocol.Dispatcher.Dispatcher.Create<ClientImpl>();

    public DataImpl props = new DataImpl();

    public ActorClient()
    {
        this.props.SetOwner(this);
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
        server = Protocol.Sender.Sender.Create<ServerImpl>(sender);

        OnClientBinded();
    }

    public override void DispatchMessage(BinaryReader reader)
    {
        dispatcher.Dispatch((this as ClientImpl)!, reader);
    }

    public override void OnPropertyChanged(BinaryReader reader)
    {
        //        this.props.UnpackProperty(reader);
        this.props.UnpackProperty(reader);
    }

    public void OnPropertyChanged(Common.PropertyInfo info)
    {
        MethodInfo? notifier = this.GetType().GetMethod(info.notifierName);
        notifier?.Invoke(this, null);
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