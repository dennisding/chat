
using Server;
using Common;
using Protocol;
using System.Net.Security;
using System.Diagnostics;

namespace ChatServer;

class ChatServer: ActorServer<IChatClient, IChatServer>, IChatServer
{
    public ActorId roomId;

    public override void Init()
    {
    }

    public override void Finit()
    {
        
    }

    public override void OnClientBinded()
    {
        base.OnClientBinded();
        client!.ShowMessage("I'am Ready");
    }

    public void ShowMessage(string msg)
    {
        Console.WriteLine($"ChatCore.ShowMessage: {msg}");
    }

    public void NewRoom(string name)
    {
        Console.WriteLine($"NewRoom: {name}");
        Game.CreateActor("Room", (actor) => {
            RoomServer? room = actor as RoomServer;

            room!.SetName(name);

            room.Enter(this.aid);
            this.roomId = room.aid;
        });
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

        room.ActorMessage(this.aid, msg);
    }

    void LobbyMessage(string msg)
    {
        // lobby message
        Console.WriteLine($"LobbyMessage: {msg}");
    }

    public void ClientMessage(string msg)
    {
        client!.ShowMessage(msg);
    }
}