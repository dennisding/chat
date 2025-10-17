

using Server;
using Protocol;

namespace ChatServer;

class LoginCore : ActorCore<ILoginClient, ILoginCore>, ILoginCore
{
    public LoginCore(): base()
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
        client!.LoginResult(name == password);
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