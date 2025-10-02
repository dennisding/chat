
using server;
using Common;

namespace actors
{
    class LoginCore: ActorCore<ILoginClient, IActorNull>, ILoginCore
    {
        public LoginCore(): base()
        {
            // actor.client.ClientMethod()
            // actor.core.CoreMethod()
            // actor.shadow.ShadowMethod()
        }

        public override void Init()
        {
        }

        public override void Finit()
        {
            base.Finit();
        }

        public override void EnterWorld(World _world)
        {
            base.EnterWorld(_world);
            client!.LoginResult(true);
        }

        public override void LeaveWorld()
        {
            base.LeaveWorld();
        }

        // game logic
        public void Login(string name, string password)
        {
            Console.WriteLine($"Login: {name}, {password}");

            this.client!.LoginResult(name == password);
        }
    }
}