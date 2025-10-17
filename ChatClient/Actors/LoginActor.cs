
using Protocol;
using Client;

namespace ChatClient;

class LoginActor : ActorClient<ILoginClient, ILoginCore>, ILoginClient
{
    public LoginActor()
    {
    }

    public override void OnClinetBinded()
    {
        string name = "dennis";
        string password = "dennis";
        core!.Login(name, password);
    }

    public void LoginResult(bool isOk)
    {
        Console.WriteLine($"LoginActor.LoginResult: {isOk}");
    }

    public void Echo(string msg)
    {
        Console.WriteLine($"LoginActor.Echo: {msg}");
        core!.EchoBack(msg);
    }

    public void EchoBack(string msg)
    {
        Console.WriteLine($"LoginActor.EchoBack: {msg}");
    }
}