
using Common;
using Server;

namespace ChatServer
{
    class LoginCore: ActorCore<ILoginClient, IActorNull>, ILoginCore
    {

        public LoginCore(): base()
        {
        }

        public override void EnterWorld(World _world)
        {
            base.EnterWorld(_world);
        }

        public void Login(string name, string password)
        {
            Console.WriteLine($"Login: {name}, {password}");
            this.client!.LoginResult(name == password);
        }
    }
}