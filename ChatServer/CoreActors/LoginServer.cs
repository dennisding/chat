

using Server;
using Protocol;

namespace ChatServer;

class LoginServer : ActorServer<ILoginClient, ILoginServer>, ILoginServer
{
    public LoginServer(): base()
    {
    }

    public override void OnClientBinded()
    {
        client!.Echo("Message from LoginCore");
    }

    // impl ILoginCore
    public void Login(string name, string password)
    {
        Console.WriteLine($"LoginCore.Login: {name}, {password}");
        bool result = name == password;
        client!.LoginResult(result);
        if (!result)
        {
            // login failed
            DestroySelf();
            return;
        }

        Game.CreateActor("Chat", OnChatterCreated);
    }

    void OnChatterCreated(Actor actor)
    {
        Console.WriteLine($"OnChatterCreated! {actor.aid}");

        GiveClientTo(actor);
        DestroySelf();
    }

    public void Echo(string msg)
    {
        Console.WriteLine($"LoginCore.Echo: {msg}");
        client!.EchoBack(msg);
    }

    public void EchoBack(string msg)
    {
        Console.WriteLine($"LoginCore.EchoBack: {msg}");
    }
}