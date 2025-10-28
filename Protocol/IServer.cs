
using Common;

namespace Protocol;

// 每一个Actor分为client和server两部分.
// 还有很多个clone对象.
[Common.Protocol]
public interface IServer
{

    void EnterLobby(ActorId aid, string uesrName);
    void LeaveLobby(ActorId aid, string userName);
    void LobbyMessage(ActorId senderId, string userName, string msg);

    void CheckUsername(ActorId aid, string name);
    void NewRoom(ActorId aid, string userName, string roomName);
    void EnterRoom(ActorId aid, string userName, string roomName);
    void LeaveRoom(ActorId roomId, ActorId aid, string userName);
}