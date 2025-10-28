
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
        //        Console.WriteLine($"ActorClient.ShowMessage, {msg}");
        Console.WriteLine($"--- {msg}");
    }

    // process command
    public void CommandChatMessage(string msg)
    {
        server!.ChatMessage(msg);
    }

    public void CommandNewRoom(string roomName)
    {
        server!.NewRoom(roomName);
    }

    public void CommandEnterRoom(string roomName)
    {
        server!.EnterRoom(roomName);
    }
}