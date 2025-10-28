
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
    const int MAX_NUMBER = 100;

    public void SetName(string name)
    {
        this.name = name;
    }

    public void Enter(ActorId senderId, string name)
    {
//        Console.WriteLine($"EnterWorld! {senderId}");

        if (actors.Contains(senderId) )
        {
            SendMessage(senderId, "请不要重复进入聊天室");
            return;
        }

        if (actors.Count >= MAX_NUMBER)
        {
            SendMessage(senderId, "聊天室人数已满");
            return;
        }


        actors.Add(senderId);

        IChatServer chatter = Game.GetActor<IChatServer>(senderId)!;

        OnChatterEntered(chatter, senderId, name);
    }

    void OnChatterEntered(IChatServer chatter, ActorId senderId, string userName)
    {
        chatter.OnEnterRoom(this.aid);
        // 给自己发送进入房间消息
        string msg = $"你已经进入房间[{this.name}]";
        chatter.ClientMessage(msg);

        string enterMsg = $"用户[{userName}]已经进入房间[{this.name}]";

        BroadcastMessage(enterMsg, senderId);
    }

    public void Leave(ActorId aid, string userName)
    {
//        Console.WriteLine($"LeaveWorld! {aid}");
        actors.Remove(aid);

        OnChatterLeave(aid, userName);

        if ((this.name != "大厅") && (actors.Count == 0))
        {
            Console.WriteLine($"房间[{name}]已经销毁.");
            DestroySelf();
        }
    }

    void OnChatterLeave(ActorId senderId, string userName)
    {
        SendMessage(senderId, $"你已经离开房间[{this.name}]");

        string msg = $"用户[{userName}]已经离开房间!";
        BroadcastMessage(msg, senderId);
    }

    void BroadcastMessage(string msg, ActorId senderId = new ActorId())
    {
        foreach (var aid in actors)
        {
            if (aid == senderId)
            {
                continue;
            }
            var friend = Game.GetActor<IChatServer>(aid);

            friend!.ClientMessage(msg);
        }
    }

    void SendMessage(ActorId aid, string msg)
    {
        var chatter = Game.GetActor<IChatServer>(aid);
        if (chatter != null)
        {
            chatter.ClientMessage(msg);
        }
    }

    public void ActorMessage(ActorId senderId, string userName, string msg)
    {
        // send message to self
        string selfMsg = $"[我]: {msg}";

        IChatServer self = Game.GetActor<IChatServer>(senderId)!;
        self.ClientMessage(selfMsg);

        string broadcastMsg = $"[{userName}]:{msg}";
        BroadcastMessage(broadcastMsg, senderId);
    }

    public string GetName()
    {
        return this.name;
    }
}