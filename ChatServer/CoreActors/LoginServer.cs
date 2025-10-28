

using Server;
using Protocol;

namespace ChatServer;

class LoginServer : ActorServer<ILoginClient, ILoginServer>, ILoginServer
{
    string name = "";
    public LoginServer(): base()
    {
    }

    ServerActor Server
    {
        get { return Game.GetServer<ServerActor>();}
    }

    public override void OnClientBinded()
    {
//        client!.Echo("Message from LoginCore");
    }

    // impl ILoginCore
    public void Login(string name, string password)
    {
        Console.WriteLine($"LoginCore.Login: {name}, {password}");
        bool result = name == password;
        // 密码检查
        if (!result)
        {
            client!.LoginResult(false);
            DestroySelf();
            return;
        }

        this.name = name;

        Server.CheckUsername(this.aid, name);
    }

    public void CheckUsernameResult(bool isOk)
    {
        Console.WriteLine($"CheckUsernameResult: {isOk}");

        client!.LoginResult(isOk);

        if (isOk)
        {
            Game.CreateActor("Chat", OnChatterCreated);
        }
        else
        {
            DestroySelf();
        }
    }

    void OnChatterCreated(Actor actor)
    {
        ChatServer chatter = (ChatServer)actor;
        chatter.SetName(this.name);

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