
using Common;

namespace Client
{
    public class Game
    {
        static ActorMgr actorMgr = new ActorMgr();

        public Game()
        {
        }

        public static void Init()
        {

        }

        public static Actor? GetActor(ActorId aid)
        {
            return actorMgr!.GetActor(aid);
        }

        public static ActorId CreateActor(string type)
        {

            return new ActorId(0);
        }

        public static ActorId CreateActor(string type, ActorId aid)
        {
            return actorMgr.CreateActor(type, aid);
        }

        public static void RegisterActor(string name, Type type)
        {
            actorMgr.RegisterActor(name, type);
        }
    }
}