
//global using ActorId = long;

using Common;
using System.Runtime.CompilerServices;
using Utils;

namespace Server;

public class Game
{
    static ActorMgr? actorMgr;
    static string defaultActor = "";
    static ActorId coreId = new ActorId();

    public static Config config = new Config();

//    static MailboxMgr mailboxMgr = new MailboxMgr();
    static Uuid serverId = Utils.Utils.GenUuid();

    public static void Init(Config? cfg = null)
    {
        if (cfg != null)
        {
            config = cfg;
        }
        actorMgr = new ActorMgr();
    }

    public static void Update()
    {

    }

    public static Actor? GetActor(ActorId aid)
    {
        return actorMgr!.GetActor(aid);
    }

    public static T? GetActor<T>(ActorId aid)
    {
        return (T?)(object?)GetActor(aid);
    }

    public static IEnumerable<T> GetActors<T>()
    {
        return actorMgr!.GetActors<T>();
    }

    public static ActorId CreateActor(string name)
    {
        Console.WriteLine($"CreateActor, {name}");

        Actor actor = actorMgr!.CreateActor(name);

        return actor.aid;
    }

    public static ActorId CreateActor(string name, Action<Actor> callback)
    {
        return actorMgr!.CreateActor(name, callback);
    }

    public static ActorId CreateDefaultActor(Action<Actor> callback)
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

    public static void SetServerId(ActorId aid)
    {
        coreId = aid;
    }

    public static T GetServer<T>()
    {
        return Game.GetActor<T>(coreId)!;
    }

    //// mailbox
    //public static void RegisterMailbox(Uuid serverId, IMailSender sender)
    //{
    //    mailboxMgr.RegisterMailSender(serverId, sender);
    //}

    //public static void UnregisterMailbox(Uuid serverId)
    //{
    //    mailboxMgr.UnregisterMailSender(serverId);
    //}

    //public static void Mail(Mailbox mailbox, MemoryStream stream)
    //{
    //    if (mailbox.serverId == serverId)
    //    {
    //        // dispatch the message to actor!!
    //    }
    //    else
    //    {
    //        mailboxMgr.Mail(mailbox, stream);
    //    }
    //}
}
