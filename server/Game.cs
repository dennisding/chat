
//global using ActorId = long;

namespace server
{
    public class Game
    {
        static ActorMgr? actorMgr;

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
            ActorId aid = GenActorId();

            Console.WriteLine($"CreateActor, {name}, {aid}");

            Actor actor = actorMgr!.CreateActor(name, aid);

            return aid;
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
            return new  ActorId((timeStamp << 32) & rand);
        }
    }
}