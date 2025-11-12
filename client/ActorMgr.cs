
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

        public void RegisterActor(string name, Type type)
        {
            actorTypes[name] = type;
        }

        public ActorId CreateActor(string name, ActorId aid, BinaryReader? props = null)
        {
            if (actorTypes.TryGetValue(name, out Type? type))
            {
//                Console.WriteLine($"do Create Actors{name}");
                Actor actor = (Actor)Activator.CreateInstance(type)!;
                actor.aid = aid;
                actors[aid] = actor;

                actor.UnpackProperty(props);

                actor.Init();
            }

            return aid;
        }

        public void DelActor(ActorId aid)
        {
            if (actors.TryGetValue(aid, out Actor? actor))
            {
                actor.Finit();
                actors.Remove(aid);
            }
            else
            {
                Console.WriteLine($"Invalid ActorId: {aid}");
            }
        }

        public Actor? GetActor(ActorId aid)
        {
            if (actors.TryGetValue(aid, out Actor? actor))
            {
                return actor;
            }
            return null;
        }
    }
}