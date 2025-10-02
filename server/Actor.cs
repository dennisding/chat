
namespace server
{
    public class Actor
    {
        public ActorId aid;
        public World? world;

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
    }

    public class ActorCore<IClient, IShadow> : Actor
    {
        public IClient? client;
        public IShadow? shadow;
        public ActorCore()
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