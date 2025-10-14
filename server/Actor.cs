
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

        public virtual void BindClient(ActorConnection client)
        {
            this.clientInfo = client;
        }

        public virtual void DispatchMessage(MemoryStream stream)
        {
        }

        public virtual void BecomePlayer()
        {
        }
    }

    public class ActorCore<IClient, IShadow> : Actor
    {
        public IClient? client;
        public IShadow? shadow;
        public ActorCore()
        {
        }

        public override void BindClient(ActorConnection client)
        {
            base.BindClient(client);
            // reset the clint and shadow
        }

        public override void BecomePlayer()
        {
        }
    }

    public class ActorShadow<IClient, ICore>: Actor
    {
        public ActorShadow()
        {

        }
    }
}