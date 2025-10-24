

using Common;

namespace Protocol;

[Common.Protocol]
public interface IRoomServer
{
    void Enter(ActorId aid, string name);
    void Leave(ActorId aid);
    void ActorMessage(ActorId aid, string userName, string msg);
}
