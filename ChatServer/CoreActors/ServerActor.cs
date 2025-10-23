
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
        lobby = actor as IRoomServer;
    }

    public void EnterLobby(ActorId aid)
    {
        Console.WriteLine($"EnterLobby: {aid}");
        lobby!.Enter(aid);
    }

    public void LeaveLobby(ActorId aid)
    {
        Console.WriteLine($"LeaveLobby: {aid}");

        lobby!.Leave(aid);
    }
}