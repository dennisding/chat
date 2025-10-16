using Common;

namespace Client
{
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

        public virtual void OnClinetBinded()
        {
        }

        public virtual void DispatchMessage(BinaryReader reader)
        {

        }
    }

    public class ActorClient<ClientImpl, ServerImpl> : Actor
        where ServerImpl: class
        where ClientImpl : class
    {
        public ServerImpl? core;
        IDispatcher<ClientImpl> dispatcher = Protocol.Dispatcher.Dispatcher.Create<ClientImpl>();

        public ActorClient()
        {
            return;
        }

        public override void BindClient(ActorServices? services) 
        {
            base.BindClient(services);

            if (services == null)
            {
                core = null;
                return;
            }

            ISender sender = new ActorSender(aid, services);
            core = Protocol.Sender.Sender.Create<ServerImpl>(sender);

            OnClinetBinded();
        }

        public override void DispatchMessage(BinaryReader reader)
        {
            dispatcher.Dispatch((this as ClientImpl)!, reader);
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
}