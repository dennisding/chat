
using Client;
using Protocol;

namespace ChatClient;

class ChatClient : ActorClient<IChatClient, IChatServer>, IChatClient
{

    public override void OnClientBinded()
    {
        Console.WriteLine("ChatActor.OnClientBinded");
    }

    public void ShowMessage(string msg)
    {
        Console.WriteLine($"ActorClient.ShowMessage, {msg}");
    }
}