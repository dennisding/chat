
using Common;

namespace Client
{
    public class ActorMgr
    {
        Dictionary<ActorId, Actor> actors;
        Dictionary<string, Type> actorTypes;
        public ActorMgr()
        {
            actors = new Dictionary<ActorId, Actor>();
            actorTypes = new Dictionary<string, Type>();
        }

        public void AddActorType(string name, Type type)
        {
            actorTypes[name] = type;
        }

        ActorId CreateActor(string name)
        {
            return new ActorId(0);
        }

        public ActorId CreateActor(string name, ActorId aid)
        {
            if (actorTypes.TryGetValue(name, out Type? type))
            {
                Actor actor = (Actor)Activator.CreateInstance(type)!;
                actor.aid = aid;
                actors[aid] = actor;
            }

            return aid;
        }
    }
}