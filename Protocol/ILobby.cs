
using Common;

namespace Protocol;

// lobby did not have client
public interface ILobbyClient
{

}

[Common.Protocol]
public interface ILobbyServer
{
    void EnterLobby(ActorId aid);
    void LeaveLobby(ActorId aid);

    void SendMessage(ActorId aid, string msg);
}
