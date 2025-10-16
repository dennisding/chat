
using Common;

namespace Server
{
    public class Actor
    {
        public ActorId aid;
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

        public virtual void DispatchMessage(MemoryStream stream)
        {
        }

        public virtual void BecomePlayer()
        {
        }
    }

    public class ActorCore<ClientImpl, CoreImpl> : Actor
        where ClientImpl: class
        where CoreImpl: class
    {
        public ClientImpl? client;
        IDispatcher<CoreImpl> dispatcher = Protocol.Dispatcher.Dispatcher.Create<CoreImpl>();

        public ActorCore()
        {
            client = null;
        }

        public override void BindClient(ActorConnection? connection)
        {
            base.BindClient(connection);
            // reset the clint and shadow
            if (connection == null)
            {
                client = null;
                return;
            }

            ISender sender = new ClientSender(aid, connection);
            client = Protocol.Sender.Sender.Create<ClientImpl>(sender);

            OnClientBinded();
        }

        public override void DispatchMessage(MemoryStream stream)
        {
            BinaryReader reader = new BinaryReader(stream);
            dispatcher.Dispatch((this as CoreImpl)!, reader);
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
}