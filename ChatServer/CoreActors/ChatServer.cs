
using Server;
using Common;
using Protocol;
using System.Net.Security;
using System.Diagnostics;

namespace ChatServer;

class ChatServer: ActorServer<IChatClient, IChatServer>, IChatServer
{
    public ActorId roomId;
    string name = "";

    ServerActor Server
    {
        get { return Game.GetServer<ServerActor>(); }
    }

    public override void Init()
    {
    }

    public override void Finit()
    {
        
    }

    public override void OnClientBinded()
    {
        base.OnClientBinded();

        Server.EnterLobby(this.aid, this.name);

        client!.ShowMessage("I'am Ready");
    }

    public void ShowMessage(string msg)
    {
        Console.WriteLine($"ChatCore.ShowMessage: {msg}");
    }

    public void NewRoom(string name)
    {
        Console.WriteLine($"NewRoom: {name}");

        Server.NewRoom(this.aid, this.name, name);
    }

    public void NewRoomResult(bool isOk, ActorId roomId)
    {
        if (isOk)
        {
            this.client!.ShowMessage("房间创建成功");
            //IRoomServer room = Game.GetActor<IRoomServer>(roomId)!;
            //room.Enter(this.aid, this.name);
        }
        else
        {
            this.client!.ShowMessage("无法创建房间");
        }
    }

    public void EnterRoom(string roomName)
    {
        Console.WriteLine($"EnterRoom: {roomName}");

        Server.EnterRoom(this.aid, this.name, roomName);
    }

    public void OnEnterRoom(ActorId roomId)
    {
        if (this.roomId != default)
        {
            Server.LeaveRoom(this.roomId, this.aid, this.name);
        }

        this.roomId = roomId;
    }

    public void LeaveRoom()
    {
        Console.WriteLine($"LeaveRoom");
    }

    public void ChatMessage(string msg)
    {
        Console.WriteLine($"OnChatMessage: {msg}");

        IRoomServer? room = Game.GetActor<IRoomServer>(this.roomId);

        if (room == null)
        {
            // we are not in a room
            LobbyMessage(msg);
            return;
        }

        room.ActorMessage(this.aid, this.name, msg);
    }

    void LobbyMessage(string msg)
    {
        // lobby message
        Console.WriteLine($"LobbyMessage: {msg}");

        Server.LobbyMessage(this.aid, this.name, msg);
    }

    public void ClientMessage(string msg)
    {
        client!.ShowMessage(msg);
    }

    public void SetName(string name)
    {
        this.name = name;
    }
}