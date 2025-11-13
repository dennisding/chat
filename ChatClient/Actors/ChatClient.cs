
using Client;
using Protocol;

namespace ChatClient;

class ChatClient : ActorClient<IChatClient, IChatServer, ChatData>, IChatClient
{
    public override void Init()
    {
        Console.WriteLine($"UserName: {this.props.name}");
    }


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

    public void SendData(ChatData data)
    {
//        Item item = new Item();
        Console.WriteLine($"ChatClient.SendData: {data.hp}, {data.name}, {data.friend}");

        ClientChatData? clientData = data as ClientChatData;
        if (clientData != null)
        {
            Console.WriteLine($"ClientChatData: {clientData.iv}");
            clientData.Use();
        }
    }
}

class ClientChatData : ChatData
{
    public int iv = 100;
    public ClientChatData()
    {

    }

    public void Use()
    {
        Console.WriteLine("ClientChatData use!!!!");
    }
}