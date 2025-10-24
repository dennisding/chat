
using Common;
using Server;
using Protocol;

namespace ChatServer;

class ServerActor : ActorServer<IActorNull, IServer>, IServer
{

    // lobby
    IRoomServer? lobby;

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
//        lobby = actor as IRoomServer;
        lobby = (IRoomServer)actor;
    }

    public void EnterLobby(ActorId aid, string userName)
    {
        Console.WriteLine($"EnterLobby: {aid}");
        lobby!.Enter(aid, userName);
    }

    public void LeaveLobby(ActorId aid)
    {
        Console.WriteLine($"LeaveLobby: {aid}");

        lobby!.Leave(aid);
    }

    public void LobbyMessage(ActorId senderId, string userName, string msg)
    {
        lobby!.ActorMessage(senderId, userName, msg);
    }
}