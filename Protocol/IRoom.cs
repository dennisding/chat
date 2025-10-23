

using Common;

namespace Protocol;

[Common.Protocol]
public interface IRoomServer
{
    void Enter(ActorId aid);
    void Leave(ActorId aid);
    void ActorMessage(ActorId aid, string msg);
}
