using Services;

namespace client
{
    public class Actor
    {
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
    }

    public class ActorClient<IServer>: Actor
    {
        public IServer? core;

        public ActorClient()
        {

        }
    }
}