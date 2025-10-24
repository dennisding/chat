
using Client;
using Protocol;

namespace ChatClient;

// new {room_name}
// enter {room_name}
// leave
// quit
// to {actor_name} {msg}
// login {user_name} {password}
// msg {msg}

class CommandInput
{
    string[]? tokens = null;
    int index = 0;

    public string? command;
    public string remain = "";
    public CommandInput(string command)
    {
        remain = command;
        tokens = command.Split();

        this.command = NextToken();
    }

    public string NextToken()
    {
        if (tokens == null)
        {
            return "";
        }

        while (index < tokens.Length)
        {
            string token = tokens[index];
            ++index;
            string result = token.Trim();
            if (result.Length != 0)
            {
                remain = remain.Substring(token.Length);
                return result;
            }
        }

        return "";
    }
}

class CommandMgr
{
    public InputMgr inputMgr;

    HashSet<string> hasSubCommand;

    public CommandMgr()
    {
        inputMgr = new InputMgr(this);
        hasSubCommand = new HashSet<string>();

        // hashSubCommand
        RegisterSubCommands();
    }

    void RegisterSubCommands()
    {
        hasSubCommand.Add("new");
    }

    public void Tick()
    {
        inputMgr.Tick();
    }

    public void InputCommand(string command)
    {
        Console.WriteLine($"InputCommand: {command}");
        
        CommandInput input = new CommandInput(command);

        ProcessCommand(input);
    }

    void ProcessCommand(CommandInput input)
    {
        input.remain = input.remain.Trim();
        if (input.command == "new")
        {
            NewRoom(input);
        }
        else if (input.command == "enter")
        {
            EnterRoom(input);
        }
        else if (input.command == "leave")
        {
            Leave();
        }
        else if (input.command == "quit")
        {
            Quit();
        }
        else if (input.command == "to")
        {
            To(input);
        }
        else if (input.command == "login")
        {
            Login(input);
        }
        else if (input.command == "msg")
        {
            Msg(input);
        }
    }

    public void NewRoom(CommandInput command)
    {
        string roomName = command.remain;
        Console.WriteLine($"Create new room: {roomName}");

        var player = Game.GetPlayer<ChatClient>();
        if (player == null)
        {
            Console.WriteLine("请先登录.");
            return;
        }
        player!.server!.NewRoom(roomName);
    }

    public void EnterRoom(CommandInput command)
    {
        Console.WriteLine($"EnterRoom: {command.remain}");

        var player = Game.GetPlayer<ChatClient>();
    }

    public void Leave()
    {
        Console.WriteLine($"Command.Leave");
    }

    public void Quit()
    {
        Console.WriteLine($"Command.Quit");
        Game.running = false;
    }

    public void To(CommandInput command)
    {
        Console.WriteLine($"Command.To: {command.remain}");
    }

    public void Login(CommandInput command)
    {
        string remain = command.remain;
        Console.WriteLine($"Command.Login: {remain}");
        string[] tokens = remain.Split();

        string username = tokens[0];
        string password = tokens[1];

        // var player = Game.GetPlayer<ChatClient>();
        var player = Game.GetPlayer<LoginClient>();
        player!.server!.Login(username, password);
    }

    public void Msg(CommandInput command)
    {
        string remain = command.remain;
        Console.WriteLine($"Command.msg: {remain}");

        var player = Game.GetPlayer<ChatClient>();
        player!.server!.ChatMessage(remain);
    }
}