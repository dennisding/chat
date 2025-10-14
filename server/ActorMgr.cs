
namespace Server
{
    class ActorMgr
    {
        public Dictionary<string, Type> types;
        public Dictionary<ActorId, Actor> actors;

        public ActorMgr()
        {
            types = new Dictionary<string, Type>();
            actors = new Dictionary<ActorId, Actor>();
        }

        public void RegisterActor(string name, Type type)
        {
            types[name] = type;
        }

        public Actor? GetActor(ActorId aid)
        {
            if (actors.TryGetValue(aid, out Actor? actor))
            {
                return actor;
            }

            return null;
        }

        public void DelActor(ActorId aid)
        {
            Actor actor = actors[aid];
            if (actor.inWorld)
            {
                actor.LeaveWorld();
            }

            actor.Finit();
        }

        public Actor CreateActor(string name)
        {
            ActorId aid = Game.GenActorId();
            Actor actor = (Actor)Activator.CreateInstance(types[name])!;
            actor.aid = aid;

            actors[aid] = actor;

            return actor;
        }

        public ActorId CreateActor(string name, Action<string, ActorId, Actor> createCallback)
        {
            Actor actor = CreateActor(name);
            actor.aid = Game.GenActorId();
            createCallback(name, actor.aid, actor);

            return actor.aid;
        }
    }
}