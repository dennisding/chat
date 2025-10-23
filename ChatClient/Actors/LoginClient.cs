
using Protocol;
using Client;
using Common;

namespace ChatClient;

class LoginClient : ActorClient<ILoginClient, ILoginServer>, ILoginClient
{
    public LoginClient()
    {
    }

    public override void OnClientBinded()
    {
        string name = "dennis";
        string password = "dennis";
        server!.Login(name, password);
    }

    public void LoginResult(bool isOk)
    {
        Console.WriteLine($"LoginActor.LoginResult: {isOk}");
    }

    public void Echo(string msg)
    {
        Console.WriteLine($"LoginActor.Echo: {msg}");
        server!.EchoBack(msg);
    }

    public void EchoBack(string msg)
    {
        Console.WriteLine($"LoginActor.EchoBack: {msg}");
    }
}