
using Server;
using Protocol;

namespace ChatServer;


partial class ChatServer: ActorServer<IChatClient, IChatServer, Protocol.ChatData>,
    IChatServer
{
    public ActorId roomId;
    string name = "";

    ServerActor Server
    {
        get { return Game.GetServer<ServerActor>(); }
    }

    public override void Init()
    {
        props.SetOwner(this);
    }

    public void _hp_Changed()
    {
        Console.WriteLine($"ChatActor._hp_Changed: {this.props.hp}");
    }

    public void _name_Changed()
    {
        Console.WriteLine($"ChatActor._name_Changed: {this.props.name}");
    }

    public override void Finit()
    {
        Server.LeaveRoom(true, this.roomId, this.aid, this.name);
        Server.OnUserLogout(this.aid, this.name);
    }

    public override void OnClientBinded()
    {
        base.OnClientBinded();

        Server.OnUserLogin(this.aid, this.name);
        Server.EnterLobby(this.aid, this.name);

        client!.ShowMessage("I'am Ready");

        // 以下是测试代码
        this.props.hp = 1024;
        this.props.name = "new_name";

        ChatData data = new ChatData();
        data.hp = 66666;
        data.name = "无名";

        client!.SendData(data);
    }

    public void ShowMessage(string msg)
    {
        Console.WriteLine($"ChatCore.ShowMessage: {msg}");
    }

    public void NewRoom(string roomName)
    {
        Console.WriteLine($"NewRoom: {roomName}");

        Server.NewRoom(this.aid, this.name, roomName);
    }

    public void NewRoomResult(bool isOk, ActorId roomId)
    {
        if (isOk)
        {
            this.client!.ShowMessage("房间创建成功");
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

    public void OnEnterRoom(ActorId roomId, bool isLobby)
    {
        ActorId oldRoom = this.roomId;
        this.roomId = roomId;
        if (!isLobby && (oldRoom != roomId))
        {
            Server.LeaveRoom(true, oldRoom, this.aid, this.name);
        }

        this.roomId = roomId;
    }

    public void LeaveRoom()
    {
        Console.WriteLine($"LeaveRoom");
        Server.LeaveRoom(false, this.roomId, this.aid, this.name);
        Server.EnterLobby(this.aid, this.name);
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

    public void MessageTo(string userName, string msg)
    {
        string message = $"[{this.name}] {msg}";
        Server.MessageTo(userName, message);
    }

    public void SetName(string name)
    {
        this.name = name;
    }
}