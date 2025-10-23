
using Common;
using Microsoft.VisualBasic;
using Protocol;
using Server;
using System.Security.AccessControl;

namespace ChatServer;

class RoomServer : ActorServer<IActorNull, IRoomServer>, IRoomServer
{
    string name = "";
    HashSet<ActorId> actors = new HashSet<Common.ActorId>();

    public void SetName(string name)
    {
        this.name = name;
    }

    public void Enter(ActorId aid)
    {
        Console.WriteLine($"EnterWorld! {aid}");

        actors.Add(aid);

        IChatServer chatter = Game.GetActor<IChatServer>(aid)!;

        OnChatterEntered(chatter);
    }

    void OnChatterEntered(IChatServer chatter)
    {
        // 给自己发送进入房间消息
        string msg = $"你已经进入房间[{name}]";
        chatter.ClientMessage(msg);
        
        // 给其它人发送自己已经进入房间的消息
        foreach (var aid in actors)
        {
            if (aid == this.aid)
            {
                continue;
            }

            string enterMsg = $"用户[未知]已经进入房间[{name}]";
            var friend = Game.GetActor<IChatServer>(aid);

            friend!.ClientMessage(enterMsg);
        }
    }

    public void Leave(ActorId aid)
    {
        Console.WriteLine($"LeaveWorld! {aid}");
        actors.Remove(aid);

        if (actors.Count == 0 )
        {
            Console.WriteLine($"房间[{name}]已经销毁.");
            DestroySelf();
        }
    }

    public void ActorMessage(ActorId aid, string msg)
    {
    }
}