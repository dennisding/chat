
//global using ActorId = long;

namespace Server
{
    public class Game
    {
        static ActorMgr? actorMgr;
        static string defaultActor = "";

        public static void Init()
        {
            actorMgr = new ActorMgr();
        }

        public static void Update()
        {

        }

        public static Actor? GetActor(ActorId aid)
        {
            return actorMgr!.GetActor(aid);
        }

        public static ActorId CreateActor(string name)
        {
            Console.WriteLine($"CreateActor, {name}");

            Actor actor = actorMgr!.CreateActor(name);

            return actor.aid;
        }

        public static ActorId CreateActor(string name, Action<string, ActorId, Actor> callback)
        {
            return actorMgr!.CreateActor(name, callback);
        }

        public static ActorId CreateDefaultActor(Action<string, ActorId, Actor> callback)
        {
            string name = defaultActor;
            if (name == "")
            {
                name = "Login";
            }
            return CreateActor(name, callback);
        }

        public static void DelActor(ActorId aid)
        {
            actorMgr!.DelActor(aid);
            Console.WriteLine($"DelActor: {aid}");
        }

        public static ActorId GenActorId()
        {
            // 64 位, 高32为为当前秒数, 低32位是一个随机数
            long timeStamp = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
            long rand = Random.Shared.Next();
            return new  ActorId((timeStamp << 32) | rand);
        }

        public static void RegisterActor(string name, Type type)
        {
            actorMgr!.RegisterActor(name, type);
        }

        public static void SetDefaultActor(string name)
        {
            defaultActor = name;
        }
    }
}