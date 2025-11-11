
using Client;
using Protocol;

namespace ChatClient;

class ChatClient : ActorClient<IChatClient, IChatServer, ChatData>, IChatClient
{

    public override void OnClientBinded()
    {
        Console.WriteLine("ChatActor.OnClientBinded");
    }

    public void ShowMessage(string msg)
    {
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

    public void CommandMessageTo(string userName, string msg)
    {
        server!.MessageTo(userName, msg);
    }

    public void _hp_Changed()
    {
        Console.WriteLine($"ChatClient._hp_Changed: {this.props.hp}");
    }

    public void _name_Changed()
    {
        Console.WriteLine($"ChatClient._name_Changed: {this.props.name}");
    }
}