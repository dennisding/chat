using Common;
using Services;

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

        public virtual void DispatchMessage(BinaryReader reader)
        {

        }
    }
}