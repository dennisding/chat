
using Common;

namespace Protocol;

// 每一个Actor分为client和server两部分.
// 还有很多个clone对象.
[Common.Protocol]
public interface IServer
{

    void EnterLobby(ActorId aid, string uesrName);
    void LeaveLobby(ActorId aid);
    void LobbyMessage(ActorId senderId, string userName, string msg);
}