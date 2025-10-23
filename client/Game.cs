
using Common;

namespace Client
{
    public class Game
    {
        static ActorMgr actorMgr = new ActorMgr();
        static ActorId playerId;

        public Game()
        {
        }

        public static void Init()
        {

        }

        public void SetPlayer(ActorId aid)
        {
            playerId = aid;
        }

        public static Actor? GetPlayer()
        {
            return GetActor(playerId);
        }

        public static T? GetPlayer<T>()
        {
            return (T?)(object?)GetPlayer();
        }

        public static Actor? GetActor(ActorId aid)
        {
            return actorMgr!.GetActor(aid);
        }

        public static T? GetActor<T>(ActorId aid)
            where T : class
        {
            return (T?)(object?)GetActor(aid);
        }

        public static ActorId CreateActor(string type)
        {

            return new ActorId(0);
        }

        public static ActorId CreateActor(string type, ActorId aid)
        {
            return actorMgr.CreateActor(type, aid);
        }

        public static void DelActor(ActorId aid)
        {
            actorMgr.DelActor(aid);
        }

        public static void RegisterActor(string name, Type type)
        {
            actorMgr.RegisterActor(name, type);
        }
    }
}