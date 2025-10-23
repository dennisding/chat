
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
            NewRoom(input.remain);
        }
        else if (input.command == "enter")
        {
            EnterRoom(input.remain);
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
            To(input.remain);
        }
        else if (input.command == "login")
        {
            Login(input.remain);
        }
        else if (input.command == "msg")
        {
            Msg(input.remain);
        }
    }

    public void NewRoom(string remain)
    {
        string roomName = remain;
        Console.WriteLine($"Create new room: {roomName}");

        var player = Game.GetPlayer<ChatClient>();
        if (player == null)
        {
            Console.WriteLine("请先登录.");
            return;
        }
        player!.server!.NewRoom(roomName);
    }

    public void EnterRoom(string remain)
    {
        Console.WriteLine($"EnterRoom: {remain}");

        var player = Game.GetPlayer<ChatClient>();
    }

    public void Leave()
    {
        Console.WriteLine($"Command.Leave");
    }

    public void Quit()
    {
        Console.WriteLine($"Command.Quit");
    }

    public void To(string remain)
    {
        Console.WriteLine($"Command.To: {remain}");
    }

    public void Login(string remain)
    {
        Console.WriteLine($"Command.Login: {remain}");
    }

    public void Msg(string remain)
    {
        Console.WriteLine($"Command.msg: {remain}");

        var player = Game.GetPlayer<ChatClient>();
        player!.server!.ChatMessage(remain);
    }
}