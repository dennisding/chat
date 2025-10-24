
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

    public void Enter(ActorId senderId, string name)
    {
        Console.WriteLine($"EnterWorld! {senderId}");

        actors.Add(senderId);

        IChatServer chatter = Game.GetActor<IChatServer>(senderId)!;

        OnChatterEntered(chatter, senderId, name);
    }

    void OnChatterEntered(IChatServer chatter, ActorId senderId, string user_name)
    {
        // 给自己发送进入房间消息
        string msg = $"你已经进入房间[{this.name}]";
        chatter.ClientMessage(msg);
        
        // 给其它人发送自己已经进入房间的消息
        foreach (var aid in actors)
        {
            if (aid == senderId)
            {
                continue;
            }

            string enterMsg = $"用户[{user_name}]已经进入房间[{this.name}]";
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

    public void ActorMessage(ActorId aid, string userName, string msg)
    {
        string result = $"[{userName}]:{msg}";
        foreach (var actorId in actors)
        {
            var actor = Game.GetActor<IChatClient>(actorId);
            if (actor != null)
            {
                actor.ShowMessage(msg);
            }
        }
    }
}