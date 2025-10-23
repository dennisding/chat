
using Common;
using Protocol;
using Server;

namespace ChatServer;

class LobbyActor: ActorServer<IActorNull, ILobbyServer>, ILobbyServer
{
    HashSet<ActorId> actors = new HashSet<Common.ActorId>();

    public LobbyActor()
    {
    }

    public void EnterLobby(ActorId aid)
    {
        actors.Add(aid);
    }

    public void LeaveLobby(ActorId aid)
    {
        actors.Remove(aid);
    }

    public void SendMessage(ActorId sender, string msg)
    {
        // send the message to all clients
        foreach (ActorId aid in actors)
        {
        }
    }
}