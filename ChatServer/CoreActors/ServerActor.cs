
using Common;
using Server;
using Protocol;
using Microsoft.VisualBasic;

namespace ChatServer;

class ServerActor : ActorServer<IActorNull, IServer>, IServer
{

    // lobby
    //    IRoomServer? lobby;
    ActorId lobbyId = default;
    HashSet<string> usernames = new HashSet<string>();
    // HashSet<string> roomNames = new HashSet<string>();
    Dictionary<string, ActorId> roomNames = new Dictionary<string, ActorId>();

    Dictionary<string, IRoomServer> rooms = new Dictionary<string, IRoomServer>();

    public ServerActor()
    {

    }

    public override void Init()
    {
        Game.CreateActor("Room", OnLobbyCreated);
    }

    void OnLobbyCreated(Actor actor)
    {
        var room = actor as RoomServer;
        room!.SetName("大厅");
        lobbyId = room.aid;
//        lobby = (IRoomServer)actor;
    }

    public void EnterLobby(ActorId aid, string userName)
    {
        usernames.Add(userName);
        //Console.WriteLine($"EnterLobby: {aid}");
        //lobby!.Enter(aid, userName);
        IRoomServer? lobby = Game.GetActor<IRoomServer>(this.lobbyId);

        lobby!.Enter(aid, userName);
    }

    public void LeaveLobby(ActorId aid, string userName)
    {
        Console.WriteLine($"LeaveLobby: {aid}, {userName}");

        this.usernames.Remove(userName);
        //        lobby!.Leave(aid, userName);
        IRoomServer? lobby = Game.GetActor<IRoomServer>(this.lobbyId);
        lobby!.Leave(aid, userName);
    }

    public void LobbyMessage(ActorId senderId, string userName, string msg)
    {
        //        lobby!.ActorMessage(senderId, userName, msg);
        var lobby = Game.GetActor<IRoomServer>(this.lobbyId);
        lobby!.ActorMessage(senderId, userName, msg);
    }

    public void CheckUsername(ActorId aid, string name)
    {
        bool isValid = true;
        if (usernames.Contains(name))
        {
            isValid = false;
        }

        ILoginServer loginActor = Game.GetActor<ILoginServer>(aid)!;
        loginActor.CheckUsernameResult(isValid);
    }

    public void NewRoom(ActorId aid, string userName, string roomName)
    {
        if (roomNames.ContainsKey(roomName))
        {
            IChatServer chatter = Game.GetActor<IChatServer>(aid)!;
            chatter.NewRoomResult(false, new Common.ActorId());
            return;
        }

        Game.CreateActor("Room", (actor) =>
        {
            RoomServer room = (RoomServer)actor;
            room.SetName(roomName);

            roomNames[roomName] = actor.aid;
            // new room created
            IChatServer? chatter = Game.GetActor<IChatServer>(aid);
            if (chatter != null)
            {
                rooms.Add(roomName, room);

                chatter.NewRoomResult(true, room.aid);

                EnterRoom(aid, userName, roomName);
            }
            else
            {
                // creator leave the world
                room.DestroySelf();
            }
        });
    }

    public void EnterRoom(ActorId aid, string userName, string roomName)
    {
        bool entered = false;
        foreach (var roomInfo in roomNames)
        {
            if (roomInfo.Key == roomName)
            {
                // enter the room
                entered = true;
                IRoomServer? room = Game.GetActor<IRoomServer>(roomInfo.Value);
                if (room != null)
                {
                    room.Enter(aid, userName);
                    break;
                }
            }
        }
        if (!entered)
        {
            string msg = $"无法进入房间[{roomName}]";
            SendMessage(aid, msg);
        }
    }

    public void LeaveRoom(bool force, ActorId roomId, ActorId aid, string userName)
    {
        if ((!force) && (roomId == this.lobbyId))
        {
            SendMessage(aid, "不能退出[大厅]");
            return;
        }

        var room = Game.GetActor<IRoomServer>(roomId);
        if (room != null)
        {
            room.Leave(aid, userName);
        }

//        EnterLobby(aid, userName);
    }

    void SendMessage(ActorId aid, string msg)
    {
        IChatServer? user = Game.GetActor<IChatServer>(aid);
        if (user != null)
        {
            user.ClientMessage(msg);
        }
    }
}